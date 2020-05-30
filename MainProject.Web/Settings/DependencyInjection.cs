namespace MainProject.Web
{
    using System.Linq;
    using MainProject.Context.Context;
    using MainProject.Services.Common;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class Settings
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped(provider =>
            {
                var actionContext = provider.GetService<IActionContextAccessor>().ActionContext;
                return provider.GetService<IUrlHelperFactory>().GetUrlHelper(actionContext);
            });

            #region REGISTERING SERVICES DYNAMICALLY
            var type = typeof(BaseService);
            type.Assembly.ExportedTypes
                .Where(t => t.IsSubclassOf(type))
                .ToList().ForEach(implementationType =>
                {
                    var serviceType = implementationType.GetInterfaces()
                        .FirstOrDefault(i => typeof(IService).IsAssignableFrom(i));
                    services.AddScoped(serviceType, implementationType);
                });
            #endregion
        }
    }
}
