namespace MainProject.Core.Handlers.Users.DeleteRegisteredUserRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using MainProject.Context.Context;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class DeleteRegisteredUserRequestHandler : BaseRequestHandler<DeleteRegisteredUserRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContext _dbContext;

        public DeleteRegisteredUserRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _dbContext = provider.GetService<IDbContext>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(DeleteRegisteredUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
            {
                throw new CustomException("El usuario que está intentando eliminar no existe.");
            }

            user.IsDisabled = true;
            user.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
            return new SuccessResponse();
        }

        public override void Dispose()
        {
            _userManager?.Dispose();
            _dbContext?.Dispose();
        }
    }
}
