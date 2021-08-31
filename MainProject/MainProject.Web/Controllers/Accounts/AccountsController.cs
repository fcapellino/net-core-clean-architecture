namespace MainProject.Web.Controllers.Accounts
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using MainProject.Infrastructure.Handlers.Accounts.ChangePasswordRequestHandler;
    using MainProject.Infrastructure.Handlers.Accounts.EstablishNewPasswordRequestHandler;
    using MainProject.Infrastructure.Handlers.Accounts.SendRecoveryEmailRequestHandler;
    using MainProject.Infrastructure.Handlers.Accounts.UserLoginRequestHandler;
    using MainProject.Infrastructure.Handlers.Accounts.UserLogOutRequestHandler;
    using MainProject.Infrastructure.Handlers.Accounts.VerifyLoginDeviceRequestHandler;
    using MainProject.Web.Custom;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using ISR = Microsoft.AspNetCore.Identity;

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = Identity.ApplicationScheme)]
    public class AccountsController : BaseController
    {
        public AccountsController(IServiceProvider provider)
            : base(provider)
        {
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", $"users");
            }

            return PartialView("login", new UserLoginRequest());
        }

        [HttpPost]
        [AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] UserLoginRequest request)
        {
            var result = await HandleRequestAsync(request);
            if (result.Error)
            {
                return PartialView("login", request);
            }
            if ((result.Resources as ISR.SignInResult).RequiresTwoFactor)
            {
                return PartialView("verifylogindevice", new VerifyLoginDeviceRequest());
            }

            return RedirectToAction("index", $"users");
        }

        [HttpPost]
        [AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyLoginDevice([FromForm] VerifyLoginDeviceRequest request)
        {
            var result = await HandleRequestAsync(request);
            if (result.Error)
            {
                return PartialView("verifylogindevice", request);
            }

            return RedirectToAction("index", $"users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut([FromForm] UserLogOutRequest request)
        {
            await HandleRequestAsync(request);
            return RedirectToAction("login", $"accounts");
        }

        [HttpPost]
        [Authorization(UserRoles.Administrator, UserRoles.Regular)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await HandleRequestAsync(request);
            return Json(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendRecoveryEmail()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", $"users");
            }
            else
            {
                return PartialView("sendrecoveryemail", new SendRecoveryEmailRequest());
            }
        }

        [HttpPost]
        [AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRecoveryEmail([FromForm] SendRecoveryEmailRequest request)
        {
            await HandleRequestAsync(request);
            return PartialView("sendrecoveryemail", request);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult EstablishNewPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", $"users");
            }
            else
            {
                return PartialView("establishnewpassword", new EstablishNewPasswordRequest());
            }
        }

        [HttpPost]
        [AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> EstablishNewPassword([FromForm] EstablishNewPasswordRequest request)
        {
            var result = await HandleRequestAsync(request);
            if (result.Error)
            {
                return PartialView("establishnewpassword", request);
            }

            return RedirectToAction("index", $"users");
        }
    }
}
