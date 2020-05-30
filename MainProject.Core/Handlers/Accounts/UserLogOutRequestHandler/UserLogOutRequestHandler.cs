namespace MainProject.Core.Handlers.Accounts.UserLogOutRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class UserLogOutRequestHandler : BaseRequestHandler<UserLogOutRequest>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserLogOutRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(UserLogOutRequest request)
        {
            await _signInManager.SignOutAsync();
            return new SuccessResponse();
        }

        public override void Dispose()
        {
        }
    }
}
