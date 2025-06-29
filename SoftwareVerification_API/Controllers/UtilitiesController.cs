using AppModels.Common;
using AppModels.Service;
using AppModels;
using BusinessLogic.Repository.IRepository;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SoftwareVerification_API.Service.IService;
using SoftwareVerification_API.Service;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace SoftwareVerification_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UtilitiesController : ControllerBase
    {
        private readonly IOtpService _otpService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private readonly ILogger<UtilitiesController> _logger;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IRecurringJobManager _jobManager;
        //private readonly Dictionary<string, (string OTP, DateTime Expiration)> _otpStore = new();

        public UtilitiesController(IOtpService otpService, IUnitOfWork unitOfWork, IEmailService emailSender, ISMSService smsService, ILogger<UtilitiesController> logger, IBackgroundJobClient jobClient, IRecurringJobManager jobManager)
        {
            _otpService = otpService;
            _unitOfWork = unitOfWork;
            _emailService = emailSender;
            _smsService = smsService;
            _logger = logger;
            _jobClient = jobClient;
            _jobManager = jobManager;
        }
        [AllowAnonymous]
        [HttpPost("TestHangFire")]
        public IResult TestHangFire()
        {
            try
            {
                _jobClient.Enqueue(SD.HangFireQueueName, () => Console.WriteLine("Fire and drop from Hangire job!!!"));
                _jobClient.Schedule(SD.HangFireQueueName, () => Console.WriteLine("Schedule 15secs from Hangire job!!!"), TimeSpan.FromSeconds(20));

                _jobManager.AddOrUpdate("every1min", SD.HangFireQueueName, () => Console.WriteLine("Recurring 60secs from Hangire job!!!"), "*/1 * * * *");

                _emailService.SendEmailAsync(new EmailRequest { Body = "Scheduled from HangFire", Subject = "Test HangFire", To = "danielwiredu@gmail.com" });
                //_jobClient.Schedule<EmailServiceMailKit>(SD.HangFireQueueName, x => x.SendEmailAsync(new EmailRequest { Body = "Scheduled from HangFire", Subject = "Test HangFire", To = "danielwiredu@gmail.com" }), TimeSpan.FromSeconds(20));

                var response = "Testing Hangfire!";
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, null, User.Identity?.Name);
                return Results.Problem();
            }
        }
        [AllowAnonymous]
        [HttpPost("TestSerilogSeq")]
        public IResult TestSerilogSeq()
        {
            try
            {
                _logger.LogInformation("Starting processing...");
                _logger.LogInformation("Processing...");
                _logger.LogInformation("Process completed!");
                _logger.LogError(null, SD.LogErrorMsg, null, null);
                var response = "Testing Hangfire!";
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, null, User.Identity?.Name);
                return Results.Problem();
            }
        }
        [HttpPost("sendsms")]
        public IActionResult SendSMS([FromBody] SMSRequest request)
        {
            try
            {
                _smsService.SendSMSAsync(request);
                return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = $"SMS sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, request, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [HttpPost("sendemail")]
        public IActionResult SendEmail([FromBody] EmailRequest request)
        {
            try
            {
                _emailService.SendEmailAsync(request);
                return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = $"Email sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, request, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
    }
}
