using System.Net.Mail;
using System.Net;
using SoftwareVerification_API.Service.IService;
using AppModels.Service;
using AppModels;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using Hangfire;
using AppModels.Common;

namespace SoftwareVerification_API.Service
{
    public class SMSService : ISMSService
    {
        private readonly IConfiguration _config;
        private readonly IBackgroundJobClient _jobClient;
        public SMSService(IConfiguration config, IBackgroundJobClient jobClient)
        {
            _config = config;
            _jobClient = jobClient;
        }
        public void SendSMSAsync(SMSRequest request)
        {
            _jobClient.Enqueue(SD.HangFireQueueName, () => SendSMS(request));
        }
        public async Task<SMSResponse> SendSMS(SMSRequest request)
        {
            if (!IsValidPhoneNumber(request.destination))
                return new SMSResponse { code = "999", message = "Invalid phone number" };

            request.username = _config.GetSection("SMSSettings:username").Value!;
            request.password = _config.GetSection("SMSSettings:password").Value!;
            request.source = _config.GetSection("SMSSettings:source").Value!;

            var client = new HttpClient();
            var content = JsonConvert.SerializeObject(request);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://deywuro.com/api/sms", bodyContent);
            //string res = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SMSResponse>(contentTemp);
                return result;
            }
            else
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ErrorModel>(contentTemp);
                throw new Exception(errorModel.ErrorMessage);
            }
        }
        static bool IsValidPhoneNumber(string phonenumber)
        {
            if (phonenumber.Length == 12 && phonenumber.StartsWith("233"))
            {
                return true;
            }
            else if (phonenumber.Length == 10 && phonenumber.StartsWith("0"))
            {
                return true;
            }
            if (phonenumber == "0000000000")
                return false;
            return false;
        }
    }
}
