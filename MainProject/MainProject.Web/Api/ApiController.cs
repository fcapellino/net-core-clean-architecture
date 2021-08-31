namespace MainProject.Web.Api
{
    using System;
    using System.Threading.Tasks;
    using MainProject.Infrastructure.Handlers.Accounts.IssueJwtTokenRequestHandler;
    using MainProject.Web.Custom;
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
        public async Task<IActionResult> IssueJwtToken([FromBody] IssueJwtTokenRequest request)
        {
            var result = await HandleRequestAsync(request);
            return Json(result);
        }
    }
}
