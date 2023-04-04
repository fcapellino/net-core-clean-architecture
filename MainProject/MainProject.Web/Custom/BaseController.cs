namespace MainProject.Web.Custom
{
    using System;
    using System.Threading.Tasks;
    using MainProject.Infrastructure.Common.BaseRequestHandler;
    using MainProject.Infrastructure.Common.ResponseTypes;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class BaseController : Controller
    {
        private readonly IMediator _mediator;

        protected BaseController(IServiceProvider provider)
        {
            _mediator = provider.GetService<IMediator>();
        }

        protected async Task<IResponse> HandleRequestAsync(BaseRequest request)
        {
            return await _mediator
                .Send(request)
                .ContinueWith(task =>
                {
                    if (task.Result.Error && !string.IsNullOrEmpty(task.Result.ErrorMessage))
                    {
                        ModelState.AddModelError("error", task.Result.ErrorMessage);
                    }
                    return task.Result;
                });
        }

        protected IActionResult RedirectToDefaultRoute()
        {
            return RedirectToRoutePermanent("default", new { controller = string.Empty, action = string.Empty });
        }
    }
}
