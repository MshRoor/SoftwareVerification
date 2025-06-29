using AppModels.Service;
using SoftwareVerification_API.Service.IService;
using MailKit.Net.Smtp;
using MimeKit;
using Hangfire;
using AppModels.Common;

namespace SoftwareVerification_API.Service
{
    public class EmailServiceMailKit : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IBackgroundJobClient _jobClient;
        public EmailServiceMailKit(IConfiguration config, IBackgroundJobClient jobClient) 
        {
            _config = config;
            _jobClient = jobClient;
        }
        public void SendEmailAsync(EmailRequest request)
        {
            _jobClient.Enqueue(SD.HangFireQueueName, () => SendEmail(request));
        }
        public Task SendEmail(EmailRequest request)
        {
            var email = new MimeMessage();
            var senderName = _config.GetSection("EmailSettings:SenderName").Value;
            var senderEmail = _config.GetSection("EmailSettings:Username").Value;
            email.From.Add(new MailboxAddress(senderName, senderEmail));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(_config.GetSection("EmailSettings:Host").Value, 8889, MailKit.Security.SecureSocketOptions.Auto);
            smtp.Authenticate(_config.GetSection("EmailSettings:Username").Value, _config.GetSection("EmailSettings:Password").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
            return Task.CompletedTask;
        }
    }
}
