namespace MainProject.Web
{
    using System.Runtime.InteropServices;
    using ElmahCore;
    using ElmahCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class Settings
    {
        public static void ConfigureErrorLogging(IConfiguration configuration, IServiceCollection services)
        {
            services.AddElmah<XmlFileErrorLog>(options =>
            {
                options.ApplicationName = "Application";
                options.Path = "errorlog";
                options.CheckPermissionAction = context =>
                {
                    return context.User.Identity.IsAuthenticated;
                };

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    options.LogPath = configuration.GetSection("Logging:Paths:Windows").Value;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    options.LogPath = configuration.GetSection("Logging:Paths:Linux").Value;
            });
        }
    }
}
