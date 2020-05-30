namespace MainProject.Web
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;

    public static partial class Settings
    {
        public static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Autenticapp Api", Version = "v1" });
                options.AddSecurityDefinition("JwtAuth", new OpenApiSecurityScheme()
                {
                    Name = "Bearer",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    Description = "Specify the authorization token.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    { new OpenApiSecurityScheme() { Reference = new OpenApiReference(){ Id = "JwtAuth", Type = ReferenceType.SecurityScheme }}, new string[] { }}
                });
            });
        }
    }
}
