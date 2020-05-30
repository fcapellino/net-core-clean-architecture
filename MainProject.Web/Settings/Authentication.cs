namespace MainProject.Web
{
    using System;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Domain.Models;
    using MainProject.Context.Context;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    public static partial class Settings
    {
        public static void ConfigureAuthentication(IConfiguration configuration, IServiceCollection services)
        {
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.User.RequireUniqueEmail = false;
                    options.User.AllowedUserNameCharacters = null;

                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;

                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();

            services
                .AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddJwtBearer(options =>
                {
                    var secret = configuration.GetSection("JwtSettings:Secret").Value;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = configuration.GetSection("JwtSettings:ValidAudience").Value,
                        ValidateAudience = configuration.GetSection("JwtSettings:ValidateAudience").Get<bool>(),
                        ValidIssuer = configuration.GetSection("JwtSettings:ValidIssuer").Value,
                        ValidateIssuer = configuration.GetSection("JwtSettings:ValidateIssuer").Get<bool>(),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret))
                    };
                });

            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services
                .ConfigureApplicationCookie(options =>
                {
                    options.Cookie.Name = "autenticapp.cookie";
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.LoginPath = "/accounts/login";
                    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                    options.SlidingExpiration = true;
                });
        }

        #region CUSTOM-CLAIMS-PRINCIPAL-FACTORY

        private class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
        {
            public CustomClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> optionsAccessor)
                : base(userManager, roleManager, optionsAccessor)
            {
            }

            protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
            {
                var identity = await base.GenerateClaimsAsync(user);
                identity.AddClaim(new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"));
                return identity;
            }
        }

        #endregion
    }
}
