namespace MainProject.Web
{
    using MainProject.Infrastructure.DataBaseContext;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static partial class Settings
    {
        public static void ConfigureDatabase(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlConnectionString"));
            });
        }
    }
}
