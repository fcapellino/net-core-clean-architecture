namespace MainProject.Services.ServicesCollection.EmailService
{
    using System.Threading.Tasks;
    using Common;

    public interface IEmailService : IService
    {
        Task SendAsync(string emailTo, string subject, string body);
    }
}
