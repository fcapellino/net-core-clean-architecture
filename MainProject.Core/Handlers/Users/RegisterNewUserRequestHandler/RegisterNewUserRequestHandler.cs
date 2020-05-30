namespace MainProject.Core.Handlers.Users.RegisterNewUserRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using MainProject.Context.Context;
    using MainProject.Services.ServicesCollection.EmailService;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class RegisterNewUserRequestHandler : BaseRequestHandler<RegisterNewUserRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IUrlHelper _urlHelper;

        public RegisterNewUserRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _dbContext = provider.GetService<IDbContext>();
            _emailService = provider.GetService<IEmailService>();
            _urlHelper = provider.GetService<IUrlHelper>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(RegisterNewUserRequest request)
        {
            var userAlreadyExists = await _dbContext.Set<ApplicationUser>()
                .AnyAsync(x => string.Equals(x.Email.ToLower(), request.Email.ToLower()));

            if (userAlreadyExists)
                throw new CustomException("El e-mail ingresado ya se encuentra en uso.");

            var newUser = new ApplicationUser()
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim().Normalize().ToLowerInvariant(),
                IsDisabled = request.IsDisabled,
                TwoFactorEnabled = true
            };

            var createResult = await _userManager.CreateAsync(newUser);
            var addToRoleResult = await _userManager.AddToRoleAsync(newUser, request.Role.ToLower());

            if (!(createResult.Succeeded && addToRoleResult.Succeeded))
                throw new InvalidOperationException();

            await SendActivationEmailAsync(newUser);
            await _dbContext.SaveChangesAsync();
            return new SuccessResponse();
        }

        private async Task SendActivationEmailAsync(ApplicationUser user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _urlHelper.Action("EstablishNewPassword", "Accounts", new { Id = user.Id.ToString(), Token = code }, _httpContext.Request.Scheme);
            var emailBody = $"Bienvenido a <strong>Auténticapp</strong>, la app de certificación de documentos institucionales digitalizados de la Universidad Tecnológica Nacional Facultad Regional San Francisco. Para establecer su contraseña y acceder al sitio haga clic en el siguiente <a target='_blank' href='{callbackUrl}'>vínculo.</a>";

            try
            {
                await _emailService.SendAsync(user.Email, "Activación de cuenta", emailBody);
            }
            catch
            {
                throw new CustomException("Ocurrió un error al intentar enviar el e-mail de activación de cuenta.");
            }
        }
        public override void Dispose()
        {
            _userManager?.Dispose();
            _dbContext?.Dispose();
            _emailService?.Dispose();
        }
    }
}
