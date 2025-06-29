namespace SoftwareVerification_API.Service.IService
{
    public interface IOtpService
    {
        string GenerateOtp();
        bool IsOtpValid(string inputOtp, string storedOtp, DateTime expirationTime);
    }
}
