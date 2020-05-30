namespace MainProject.Core.Handlers.Accounts.SendRecoveryEmailRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using MainProject.Services.ServicesCollection.EmailService;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public class SendRecoveryEmailRequestHandler : BaseRequestHandler<SendRecoveryEmailRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IUrlHelper _urlHelper;

        public SendRecoveryEmailRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _emailService = provider.GetService<IEmailService>();
            _urlHelper = provider.GetService<IUrlHelper>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(SendRecoveryEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize().ToLowerInvariant());
            if (user == null || user.IsDisabled)
            {
                throw new CustomException("El usuario especificado es inválido.");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _urlHelper.Action("EstablishNewPassword", $"Accounts", new { Id = user.Id.ToString(), Token = code }, _httpContext.Request.Scheme);
            var emailBody = $"Para restablecer su contraseña haga clic en el siguiente <a href='{callbackUrl}'>vínculo.</a>";

            try
            {
                await _emailService.SendAsync(user.Email, "Restablecer contraseña", emailBody);
            }
            catch
            {
                throw new CustomException("Ocurrió un error al intentar enviar el e-mail de recuperación de cuenta.");
            }

            return new SuccessResponse();
        }

        public override void Dispose()
        {
            _userManager?.Dispose();
            _emailService?.Dispose();
        }
    }
}
