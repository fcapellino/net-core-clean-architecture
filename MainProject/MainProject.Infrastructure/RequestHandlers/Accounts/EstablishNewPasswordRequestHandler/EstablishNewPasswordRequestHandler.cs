namespace MainProject.Infrastructure.Handlers.Accounts.EstablishNewPasswordRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class EstablishNewPasswordRequestHandler : BaseRequestHandler<EstablishNewPasswordRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EstablishNewPasswordRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(EstablishNewPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize().ToLowerInvariant());
            if (user == null || !user.Id.ToString().Equals(request.Id) || user.IsDisabled)
            {
                throw new CustomException("The specified user is invalid.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

            if (!(confirmEmailResult.Succeeded && resetPasswordResult.Succeeded))
            {
                throw new CustomException("The specified password cannot be set.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            await _signInManager.RememberTwoFactorClientAsync(user);
            return new SuccessResponse();
        }

        public override void Dispose()
        {
            _userManager?.Dispose();
        }
    }
}
