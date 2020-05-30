namespace MainProject.Web.Api
{
    using System;
    using System.Threading.Tasks;
    using CustomControllers;
    using MainProject.Core.Handlers.Accounts.IssueJwtTokenRequestHandler;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api")]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiController : BaseController
    {
        public ApiController(IServiceProvider provider)
            : base(provider)
        {
        }

        [HttpPost("issueJwtToken")]
        [AllowAnonymous]
        public async Task<IActionResult> IssueJwtToken([FromBody]IssueJwtTokenRequest request)
        {
            var result = await HandleRequestAsync(request);
            return Json(result);
        }
    }
}
