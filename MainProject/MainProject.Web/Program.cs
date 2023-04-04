namespace MainProject.Web
{
    using MainProject.Web.Custom;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                #region DEVELOPMENT ENVIRONMENT
                if (hostingContext.HostingEnvironment.IsDevelopment())
                    config.AddJsonFile(path: "appsettings.development.json", optional: false, reloadOnChange: false);
                #endregion
                #region PRODUCTION ENVIRONMENT
                if (hostingContext.HostingEnvironment.IsProduction())
                    config.AddEncryptedJsonFile(path: "appsettings.json", privateKey: "Tn7^(cCjhx^*:RQAKY+Pp/:-yN3e!(:8", optional: false, reloadOnChange: false);
                #endregion
            });
    }
}
