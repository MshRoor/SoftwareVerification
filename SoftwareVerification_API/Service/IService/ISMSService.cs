using AppModels.Service;

namespace SoftwareVerification_API.Service.IService
{
    public interface ISMSService
    {
        void SendSMSAsync(SMSRequest request);
    }
}
