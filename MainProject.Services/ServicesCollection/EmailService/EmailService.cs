namespace MainProject.Services.ServicesCollection.EmailService
{
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Common;
    using Microsoft.Extensions.Configuration;

    public class EmailService : BaseService, IEmailService
    {
        public EmailService(IConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task SendAsync(string emailTo, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var sender = _configuration["EmailSettings:SmtpUser"];
            var senderPassword = _configuration["EmailSettings:SmtpPassword"];
            var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

            var mailMessage = new MailMessage() { From = new MailAddress(sender) };
            mailMessage.To.Add(new MailAddress(emailTo));
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            var clienteSMTP = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(sender, senderPassword),
                EnableSsl = enableSsl
            };

            await clienteSMTP.SendMailAsync(mailMessage);
        }

        public override void Dispose() { }
    }
}
