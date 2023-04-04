namespace MainProject.Web
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json.Serialization;

    public static partial class Settings
    {
        public static void ConfigureMvc(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    var authorizationPolicy = new AuthorizationPolicyBuilder(new[] {
                                JwtBearerDefaults.AuthenticationScheme, IdentityConstants.ApplicationScheme })
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(authorizationPolicy));
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddRazorRuntimeCompilation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}
