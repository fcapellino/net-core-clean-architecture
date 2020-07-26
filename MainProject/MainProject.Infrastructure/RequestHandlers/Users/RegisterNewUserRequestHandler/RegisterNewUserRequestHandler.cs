namespace MainProject.Infrastructure.Handlers.Users.RegisterNewUserRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using MainProject.Infrastructure.DataBaseContext;
    using MainProject.Infrastructure.Notifications;
    using MainProject.Services.ServicesCollection.EmailService;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class RegisterNewUserRequestHandler : BaseRequestHandler<RegisterNewUserRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IUrlHelper _urlHelper;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public RegisterNewUserRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _dbContext = provider.GetService<IDbContext>();
            _emailService = provider.GetService<IEmailService>();
            _urlHelper = provider.GetService<IUrlHelper>();
            _hubContext = provider.GetService<IHubContext<NotificationsHub>>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(RegisterNewUserRequest request)
        {
            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("register-new-user-request", 000, "Performing validations...")
                .ContinueWith(t => Task.Delay(500));

            var userAlreadyExists = await _dbContext.Set<ApplicationUser>()
                .AnyAsync(x => string.Equals(x.Email.ToLower(), request.Email.ToLower()));

            if (userAlreadyExists)
                throw new CustomException("The entered email is already in use.");

            var newUser = new ApplicationUser()
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().Normalize().ToLowerInvariant(),
                IsDisabled = request.IsDisabled,
                TwoFactorEnabled = true
            };

            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("register-new-user-request", 020, "Registering...")
                .ContinueWith(t => Task.Delay(500));

            var createResult = await _userManager.CreateAsync(newUser);
            var addToRoleResult = await _userManager.AddToRoleAsync(newUser, request.Role.ToLower());

            if (!(createResult.Succeeded && addToRoleResult.Succeeded))
                throw new InvalidOperationException();

            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("register-new-user-request", 070, "Sending activation email...")
                .ContinueWith(t => Task.Delay(500));

            await SendActivationEmailAsync(newUser);
            await _dbContext.SaveChangesAsync();

            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("register-new-user-request", 100, "The user has been successfully registered.");
            return new SuccessResponse();
        }

        private async Task SendActivationEmailAsync(ApplicationUser user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _urlHelper.Action("EstablishNewPassword", "Accounts", new { Id = user.Id.ToString(), Token = code }, _httpContext.Request.Scheme);
            var emailBody = $"To set your password and access the site click on the following <a target='_blank' href='{callbackUrl}'>link.</a>";

            await _emailService.SendAsync(user.Email, "Account activation", emailBody)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        throw new CustomException("An error occurred while trying to send the account activation email.");
                });
        }
        public override void Dispose()
        {
            _userManager?.Dispose();
            _dbContext?.Dispose();
            _emailService?.Dispose();
        }
    }
}
