namespace MainProject.Core.Handlers.Users.EditRegisteredUserRequestHandler
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using MainProject.Context.Context;
    using MainProject.Services.ServicesCollection.EmailService;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public class EditRegisteredUserRequestHandler : BaseRequestHandler<EditRegisteredUserRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IUrlHelper _urlHelper;

        public EditRegisteredUserRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _dbContext = provider.GetService<IDbContext>();
            _emailService = provider.GetService<IEmailService>();
            _urlHelper = provider.GetService<IUrlHelper>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(EditRegisteredUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            if (user == null)
                throw new CustomException("El usuario especificado es inválido.");

            var userAlreadyExists = await _dbContext.Set<ApplicationUser>()
                .AnyAsync(x => string.Equals(x.Email.ToLower(), request.Email.ToLower()) && !x.Id.ToString().Equals(request.Id));

            if (userAlreadyExists)
                throw new CustomException("El e-mail ingresado ya se encuentra en uso.");

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.Email = request.Email.Trim().Normalize().ToLowerInvariant();
            user.IsDisabled = request.IsDisabled;

            var emailModified =
                _dbContext.Entry(user).Property(x => x.Email).IsModified &&
                _dbContext.Entry(user).Property(x => x.PasswordHash).CurrentValue == null;

            var updateResult = await _userManager.UpdateAsync(user);
            var oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var removeFromRoleResult = await _userManager.RemoveFromRoleAsync(user, oldRole);
            var addToRoleResult = await _userManager.AddToRoleAsync(user, request.Role.ToLower());

            if (!(updateResult.Succeeded && removeFromRoleResult.Succeeded && addToRoleResult.Succeeded))
                throw new InvalidOperationException();

            if (emailModified)
                await SendActivationEmailAsync(user);

            await _dbContext.SaveChangesAsync();
            return new SuccessResponse();
        }

        private async Task SendActivationEmailAsync(ApplicationUser user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _urlHelper.Action("EstablishNewPassword", "Accounts", new { Id = user.Id.ToString(), Token = code }, _httpContext.Request.Scheme);
            var emailBody = $"Para establecer su contraseña y acceder al sitio haga clic en el siguiente <a href='{callbackUrl}'>vínculo.</a>";

            try
            {
                await _emailService.SendAsync(user.Email, "Activación de cuenta", emailBody);
            }
            catch
            {
                throw new CustomException("Ocurrió un error al intentar enviar el e-mail de activación de cuenta.");
            }
        }
        public override void Dispose()
        {
            _userManager?.Dispose();
            _dbContext?.Dispose();
            _emailService?.Dispose();
        }
    }
}
