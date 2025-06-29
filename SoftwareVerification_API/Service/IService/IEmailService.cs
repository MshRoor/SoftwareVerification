using AppModels.Service;

namespace SoftwareVerification_API.Service.IService
{
    public interface IEmailService
    {
        void SendEmailAsync(EmailRequest request);
    }
}
