namespace MainProject.Core.Handlers.Accounts.IssueJwtTokenRequestHandler
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using ISR = Microsoft.AspNetCore.Identity;

    public class IssueJwtTokenRequestHandler : BaseRequestHandler<IssueJwtTokenRequest>
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IssueJwtTokenRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _configuration = provider.GetService<IConfiguration>();
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(IssueJwtTokenRequest request)
        {
            var applicationUser = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize().ToLowerInvariant());
            var result = new ISR.SignInResult();

            if (applicationUser != null)
            {
                if (!applicationUser.EmailConfirmed || applicationUser.IsDisabled)
                {
                    throw new CustomException("El usuario no posee permisos suficientes para acceder.");
                }

                result = await _signInManager.CheckPasswordSignInAsync(applicationUser, request.Password.Trim(), lockoutOnFailure: false);
            }

            if (!result.Succeeded)
            {
                throw new CustomException("El usuario o la contraseña son incorrectos.");
            }

            var userRoles = await _userManager.GetRolesAsync(applicationUser);
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, applicationUser?.Id.ToString()) });
            claimsIdentity.AddClaims(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration.GetSection("JwtSettings:Secret").Value;
            var expirationTime = _configuration.GetSection("JwtSettings:ExpirationTime").Get<int>();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.UtcNow.AddHours(expirationTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)), SecurityAlgorithms.HmacSha256Signature),
                Subject = claimsIdentity
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new SuccessResponse(new { Token = tokenHandler.WriteToken(token) });
        }

        public override void Dispose()
        {
            _userManager?.Dispose();
        }
    }
}
