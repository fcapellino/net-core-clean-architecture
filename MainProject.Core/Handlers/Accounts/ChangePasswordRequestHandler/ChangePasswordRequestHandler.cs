namespace MainProject.Core.Handlers.Accounts.ChangePasswordRequestHandler
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class ChangePasswordRequestHandler : BaseRequestHandler<ChangePasswordRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ChangePasswordRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(ChangePasswordRequest request)
        {
            var currentUserId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var result = await _userManager.ChangePasswordAsync(currentUser, request.OldPassword.Trim(), request.NewPassword.Trim());

            if (!result.Succeeded)
            {
                throw new CustomException("Los datos ingresados no son válidos.");
            }

            await _signInManager.SignInAsync(currentUser, isPersistent: false);
            return new SuccessResponse();
        }

        public override void Dispose()
        {
            _userManager?.Dispose();
        }
    }
}