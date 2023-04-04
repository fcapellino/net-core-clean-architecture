namespace MainProject.Infrastructure.Notifications
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class NotificationsHub : Hub, ITypedNotificationsHub
    {
        public async Task AssociateTask(Guid taskId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, taskId.ToString());
        }
    }
}
