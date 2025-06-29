using System.Net.Mail;
using System.Net;
using SoftwareVerification_API.Service.IService;
using AppModels.Service;
using Hangfire;
using AppModels.Common;

namespace SoftwareVerification_API.Service
{
    public class EmailService : IEmailService
    {
        private readonly IBackgroundJobClient _jobClient;
        public EmailService(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }
        public void SendEmailAsync(EmailRequest request)
        {
            _jobClient.Enqueue(SD.HangFireQueueName, () => SendEmail(request));
            //SendEmail(request);
        }
        public Task SendEmail(EmailRequest request)
        {
            var client = new SmtpClient("mail5011.site4now.net", 465)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("noreply@xcelisolutions.com", "NOP25#Xcel@1234")
            };

            return client.SendMailAsync(
                new MailMessage(from: "noreply@xcelisolutions.com",
                                to: request.To,
                                request.Subject,
                                request.Body
                                ));

        }
    }
}
