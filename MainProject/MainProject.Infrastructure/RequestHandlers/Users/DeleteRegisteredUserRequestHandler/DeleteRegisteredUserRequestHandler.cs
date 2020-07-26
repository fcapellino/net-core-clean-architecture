namespace MainProject.Infrastructure.Handlers.Users.DeleteRegisteredUserRequestHandler
{
    using System;
    using System.Threading.Tasks;
    using Common.BaseRequestHandler;
    using Common.ResponseTypes;
    using Domain.Models;
    using MainProject.Common;
    using MainProject.Infrastructure.DataBaseContext;
    using MainProject.Infrastructure.Notifications;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.DependencyInjection;

    public class DeleteRegisteredUserRequestHandler : BaseRequestHandler<DeleteRegisteredUserRequest>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbContext _dbContext;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public DeleteRegisteredUserRequestHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _dbContext = provider.GetService<IDbContext>();
            _hubContext = provider.GetService<IHubContext<NotificationsHub>>();
        }

        protected override async Task<SuccessResponse> ExecuteAsync(DeleteRegisteredUserRequest request)
        {
            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("delete-registered-user-request", 000, "Performing validations...")
                .ContinueWith(t => Task.Delay(500));

            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
            {
                throw new CustomException("The user you are trying to delete does not exist.");
            }

            user.IsDisabled = true;
            user.IsDeleted = true;

            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.Group(request.TaskId)
                .SendAsync("delete-registered-user-request", 100, "The user has been successfully deleted.");
            return new SuccessResponse();
        }

        public override void Dispose()
        {
            _userManager?.Dispose();
            _dbContext?.Dispose();
        }
    }
}
