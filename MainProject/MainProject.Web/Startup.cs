namespace MainProject.Web
{
    using FluentValidation;
    using MainProject.Infrastructure.Common.BaseRequestHandler;
    using MainProject.Infrastructure.Notifications;
    using MainProject.Web.Custom;
    using MediatR;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            Settings.ConfigureMvc(services);
            Settings.ConfigureDatabase(_configuration, services);

            services.AddMediatR(typeof(BaseRequestHandler<>))
                .AddSignalR();

            services.AddWebOptimizer(options =>
                {
                    options.MinifyCssFiles("views/**/**/*.css");
                    options.MinifyJsFiles("views/**/**/*.js");
                });

            AssemblyScanner.FindValidatorsInAssemblyContaining<BaseRequest>()
                .ForEach(pair =>
                {
                    services.Add(ServiceDescriptor.Scoped(pair.InterfaceType, pair.ValidatorType));
                });

            Settings.ConfigureAuthentication(_configuration, services);
            Settings.ConfigureErrorLogging(_configuration, services);
            Settings.ConfigureSwagger(services);
            Settings.RegisterServices(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyMethod();
                options.AllowAnyHeader();
            });

            app.UseStatusCodePages();
            app.UseExceptionHandlerMiddleware();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseErrorLogging();
            app.UseWebOptimizer();
            app.UseStaticFiles();
            app.UseSwagger();

            app.UseEndpoints(options =>
            {
                options.MapHub<NotificationsHub>("/notificationshub");
                options.MapControllerRoute(name: "default", pattern: "{controller=accounts}/{action=login}");
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "application.api");
            });
        }
    }
}
