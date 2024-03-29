﻿namespace MainProject.Infrastructure.Handlers.Accounts.VerifyLoginDeviceRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using MainProject.Common;
    using MainProject.Domain.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using ISR = Microsoft.AspNetCore.Identity;

    public class VerifyLoginDeviceRequestHandler : BaseRequestHandler<VerifyLoginDeviceRequest>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public VerifyLoginDeviceRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(VerifyLoginDeviceRequest request)
        {
            ISR.SignInResult result = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, request.VerificationCode.Trim(), isPersistent: false, rememberClient: true);
            if (!result.Succeeded)
            {
                throw new CustomException("Verification request is invalid.");
            }

            return new SuccessResponse();
        }

        public override void Dispose()
        {
        }
    }
}
