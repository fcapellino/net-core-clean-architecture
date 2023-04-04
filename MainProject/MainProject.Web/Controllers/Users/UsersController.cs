namespace MainProject.Web.Controllers.Users
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using MainProject.Infrastructure.Handlers.Users.DeleteRegisteredUserRequestHandler;
    using MainProject.Infrastructure.Handlers.Users.EditRegisteredUserRequestHandler;
    using MainProject.Infrastructure.Handlers.Users.GetUsersListRequestHandler;
    using MainProject.Infrastructure.Handlers.Users.GetUsersRoleListRequestHandler;
    using MainProject.Infrastructure.Handlers.Users.RegisterNewUserRequestHandler;
    using MainProject.Web.Custom;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(AuthenticationSchemes = Identity.ApplicationScheme)]
    public class UsersController : BaseController
    {
        public UsersController(IServiceProvider provider)
            : base(provider)
        {
        }

        [HttpGet]
        [Authorization(UserRoles.Administrator, UserRoles.Regular)]
        public IActionResult Index()
        {
            return View("index");
        }

        [HttpGet]
        [Authorization(UserRoles.Administrator, UserRoles.Regular)]
        public async Task<IActionResult> GetUsersList([FromQuery] GetUsersListRequest request)
        {
            var result = await HandleRequestAsync(request);
            return Json(result);
        }

        [HttpGet]
        [Authorization(UserRoles.Administrator, UserRoles.Regular)]
        public async Task<IActionResult> GetUsersRoleList([FromQuery] GetUsersRoleListRequest request)
        {
            var result = await HandleRequestAsync(request);
            return Json(result);
        }

        [HttpPost]
        [Authorization(UserRoles.Administrator)]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterNewUserRequest request)
        {
            var result = await HandleRequestAsync(request);
            return Json(result);
        }

        [HttpPost]
        [Authorization(UserRoles.Administrator)]
        public async Task<IActionResult> EditRegisteredUser([FromBody] EditRegisteredUserRequest request)
        {
            var result = await HandleRequestAsync(request);
            return Json(result);
        }

        [HttpPost]
        [Authorization(UserRoles.Administrator)]
        public async Task<IActionResult> DeleteRegisteredUser([FromBody] DeleteRegisteredUserRequest request)
        {
            var result = await HandleRequestAsync(request);
            return Json(result);
        }
    }
}
