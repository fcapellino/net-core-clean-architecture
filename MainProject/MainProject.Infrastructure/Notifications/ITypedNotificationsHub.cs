namespace MainProject.Infrastructure.Notifications
{
    using System;
    using System.Threading.Tasks;

    public interface ITypedNotificationsHub
    {
        Task AssociateTask(Guid taskId);
    }
}
