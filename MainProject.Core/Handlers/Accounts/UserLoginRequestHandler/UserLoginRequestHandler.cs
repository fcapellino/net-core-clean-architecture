namespace MainProject.Core.Handlers.Accounts.UserLoginRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using MainProject.Services.ServicesCollection.EmailService;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using ISR = Microsoft.AspNetCore.Identity;

    public class UserLoginRequestHandler : BaseRequestHandler<UserLoginRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public UserLoginRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
            _emailService = provider.GetService<IEmailService>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(UserLoginRequest request)
        {
            var applicationUser = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize().ToLowerInvariant());
            if (applicationUser != null && (!applicationUser.EmailConfirmed || applicationUser.IsDisabled))
            {
                throw new CustomException("El usuario no posee permisos suficientes para acceder.");
            }

            ISR.SignInResult result = await _signInManager.PasswordSignInAsync(applicationUser?.UserName ?? string.Empty, request.Password.Trim(), isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                if (result.RequiresTwoFactor)
                {
                    #region SEND-DEVICE-VERIFICATION-EMAIL
                    var code = await _userManager.GenerateTwoFactorTokenAsync(applicationUser, TokenOptions.DefaultEmailProvider);
                    var emailBody = $"Para verificar su dispositivo y acceder al sitio introduzca el siguiente código: <strong>{code}</strong>";

                    try
                    {
                        await _emailService.SendAsync(applicationUser?.Email, "Verificación de dispositivo", emailBody);
                    }
                    catch
                    {
                        throw new CustomException("Ocurrió un error al intentar enviar el e-mail de verificación.");
                    }
                    #endregion
                }
                else
                {
                    throw new CustomException("El usuario o la contraseña son incorrectos.");
                }
            }

            return new SuccessResponse(result);
        }

        public override void Dispose()
        {
            _userManager?.Dispose();
            _emailService?.Dispose();
        }
    }
}
