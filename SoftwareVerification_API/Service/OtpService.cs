using SoftwareVerification_API.Service.IService;
using System.Security.Cryptography;
//using System.Security.Cryptography;

namespace SoftwareVerification_API.Service
{
    public class OtpService : IOtpService
    {
        //private readonly TimeSpan _otpValidityDuration = TimeSpan.FromMinutes(5);

        //public string GenerateOtp()
        //{
        //    using var rng = new RNGCryptoServiceProvider();
        //    var buffer = new byte[6];
        //    rng.GetBytes(buffer);
        //    return new Random(BitConverter.ToInt32(buffer, 0)).Next(100000, 999999).ToString();
        //}
        public string GenerateOtp()
        {
            // Generate a random 6-digit OTP
            int otp = RandomNumberGenerator.GetInt32(100000, 1000000);
            return otp.ToString();
        }

        public bool IsOtpValid(string inputOtp, string storedOtp, DateTime expirationTime)
        {
            return inputOtp == storedOtp && DateTime.UtcNow <= expirationTime;
        }
    }
}
