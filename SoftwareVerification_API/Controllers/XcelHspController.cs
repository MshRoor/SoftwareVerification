using AppModels;
using AppModels.Common;
using AppModels.XcelHsp;
using BusinessLogic.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;

namespace SoftwareVerification_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class XcelHspController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<XcelHspController> _logger;
        public XcelHspController(IUnitOfWork unitOfWork, ILogger<XcelHspController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        [HttpGet("GetProviderList")]
        public async Task<IActionResult> GetProviderList(int SearchLimit = 1000, string SearchValue = "")
        {
            try
            {
                var response = await _unitOfWork.XcelHspSite.GetProviderList(SearchLimit, SearchValue);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, null, User.Identity?.Name);
                return BadRequest(new ErrorModel { StatusCode = StatusCodes.Status400BadRequest, ErrorMessage= "An error occurred while performing the operation."});
            }
        }
        [HttpGet("GetInsurerList")]
        public async Task<IActionResult> GetInsurerList(int SearchLimit = 1000, string SearchValue = "")
        {
            try
            {
                var response = await _unitOfWork.XcelHspSite.GetInsurerList(SearchLimit, SearchValue);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, null, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [HttpGet("GetUserAccountTypes")]
        public async Task<IActionResult> GetUserAccountTypes()
        {
            try
            {
                var response = await _unitOfWork.XcelHspSite.GetUserAccountTypes();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, null, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [HttpGet("GetProviderListByIds/{InsurerId}/{ProviderIds}")]
        public async Task<IActionResult> GetProviderListByIds(string InsurerId, string ProviderIds)
        {
            if (string.IsNullOrWhiteSpace(ProviderIds) || ProviderIds.ToLower() == "undefined" || ProviderIds.ToLower() == "null")
            {
                ProviderIds = "0"; // Defaulting to "0" if invalid or //return Enumerable.Empty<ServiceProviders>();
            }
            if (string.IsNullOrWhiteSpace(InsurerId) || InsurerId.ToLower() == "undefined")
            {
                InsurerId = "0"; // Defaulting to "0" if invalid; adjust as needed
            }
            try
            {
                var response = await _unitOfWork.XcelHspSite.GetProviderListByIds(InsurerId, ProviderIds);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, $"{InsurerId},{ProviderIds}", User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [HttpGet("GetInsurerListByIds/{InsurerIds}")]
        public async Task<IActionResult> GetInsurerListByIds(string InsurerIds)
        {
            if (string.IsNullOrWhiteSpace(InsurerIds) || InsurerIds.ToLower() == "undefined" || InsurerIds.ToLower() == "null")
            {
                InsurerIds = "0"; // Defaulting to "0" if invalid; adjust as needed
            }
            try
            {
                var response = await _unitOfWork.XcelHspSite.GetInsurerListByIds(InsurerIds);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, InsurerIds, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [HttpPost("CreateInsurer")]
        public async Task<IActionResult> CreateInsurer(CreateInsurerRequest insurer)
        {
            try
            {
                var response = await _unitOfWork.XcelHspSite.CreateInsurer(insurer);
                if (response == 1)
                {
                    return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Insurance Company Created Successfully" });
                }
                else
                {
                    return BadRequest(new ActionExecutionResponse { ActionMessage = "Creating Insurance Company Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, insurer, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [HttpPost("UpdateInsurer")]
        public async Task<IActionResult> UpdateInsurer(UpdateInsurerRequest insurer)
        {
            try
            {
                var response = await _unitOfWork.XcelHspSite.UpdateInsurer(insurer);
                if (response == 1)
                {
                    return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Insurance Company Updated Successfully" });
                }
                else
                {
                    return BadRequest(new ActionExecutionResponse { ActionMessage = "Updating Insurance Company Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, insurer, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [HttpPost("CreateProvider")]
        public async Task<IActionResult> CreateProvider(CreateProviderRequest provider)
        {
            try
            {
                var response = await _unitOfWork.XcelHspSite.CreateProvider(provider);
                if (response == 1)
                {
                    return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Service Provider Created Successfully" });
                }
                else
                {
                    return BadRequest(new ActionExecutionResponse { ActionMessage = "Creating Service Provider Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, provider, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [HttpPost("UpdateProvider")]
        public async Task<IActionResult> UpdateProvider(UpdateProviderRequest provider)
        {
            try
            {
                var response = await _unitOfWork.XcelHspSite.UpdateProvider(provider);
                if (response == 1)
                {
                    return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Service Provider Updated Successfully" });
                }
                else
                {
                    return BadRequest(new ActionExecutionResponse { ActionMessage = "Updating Service Provider Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, provider, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [AllowAnonymous]
        [HttpGet("GetUnaccreditedProviders/{InsurerId}")]
        public async Task<IActionResult> GetUnaccreditedProviders(string InsurerId)
        {
            if (string.IsNullOrWhiteSpace(InsurerId) || InsurerId.ToLower() == "undefined")
            {
                InsurerId = "0"; // Defaulting to "0" if invalid;
            }
            try
            {
                var response = await _unitOfWork.XcelHspSite.GetUnaccreditedProviders(InsurerId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, $"{InsurerId}", User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
        [AllowAnonymous]
        [HttpPost("AccreditProvider")]
        public async Task<IActionResult> AccreditProvider(AccreditProviderRequest accredit)
        {
            try
            {
                var response = await _unitOfWork.XcelHspSite.AccreditProvider(accredit);
                if (response == 1)
                {
                    return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Service Provider Accredited Successfully" });
                }
                else
                {
                    return BadRequest(new ActionExecutionResponse { ActionMessage = "Updating Service Provider Failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, accredit, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }
    }
}
