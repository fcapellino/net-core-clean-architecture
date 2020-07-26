namespace MainProject.Infrastructure.Handlers.Users.EditRegisteredUserRequestHandler
{
    using System;
    using System.Linq;
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

    public class EditRegisteredUserRequestHandler : BaseRequestHandler<EditRegisteredUserRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IUrlHelper _urlHelper;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public EditRegisteredUserRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _dbContext = provider.GetService<IDbContext>();
            _emailService = provider.GetService<IEmailService>();
            _urlHelper = provider.GetService<IUrlHelper>();
            _hubContext = provider.GetService<IHubContext<NotificationsHub>>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(EditRegisteredUserRequest request)
        {
            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("edit-registered-user-request", 000, "Performing validations...")
                .ContinueWith(t => Task.Delay(500));

            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
                throw new CustomException("The specified user is invalid.");

            var userAlreadyExists = await _dbContext.Set<ApplicationUser>()
                .AnyAsync(x => string.Equals(x.Email.ToLower(), request.Email.ToLower()) && !x.Id.ToString().Equals(request.Id));

            if (userAlreadyExists)
                throw new CustomException("The entered email is already in use.");

            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("edit-registered-user-request", 020, "Saving changes...")
                .ContinueWith(t => Task.Delay(500));

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = request.Email.Trim().Normalize().ToLowerInvariant();
            user.IsDisabled = request.IsDisabled;

            var emailModified =
                _dbContext.Entry(user).Property(x => x.Email).IsModified &&
                _dbContext.Entry(user).Property(x => x.PasswordHash).CurrentValue == null;

            var updateResult = await _userManager.UpdateAsync(user);
            var oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var removeFromRoleResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
            var addToRoleResult = await _userManager.AddToRoleAsync(user, request.Role.ToLower());

            if (!(updateResult.Succeeded && removeFromRoleResult.Succeeded && addToRoleResult.Succeeded))
                throw new InvalidOperationException();

            if (emailModified)
                await SendActivationEmailAsync(user);

            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("edit-registered-user-request", 100, "The user has been successfully edited.");
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
