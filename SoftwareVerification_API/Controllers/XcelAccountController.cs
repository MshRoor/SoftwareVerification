using AppModels.Account;
using AppModels.Common;
using BusinessLogic.Repository;
using BusinessLogic.Repository.IRepository;
using DataAccess.Data;
using SoftwareVerification_API.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftwareVerification_API.Service.IService;
using AppModels.Service;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using AppModels;
using Microsoft.AspNetCore.WebUtilities;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using NuGet.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoftwareVerification_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class XcelAccountController : ControllerBase
    {
        private readonly SignInManager<XcelAppUser> _signInManager;
        private readonly UserManager<XcelAppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly APISettings _aPISettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly ILogger<XcelAccountController> _logger;

        public XcelAccountController(SignInManager<XcelAppUser> signInManager,
            UserManager<XcelAppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<APISettings> options,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IConfiguration config,
            ILogger<XcelAccountController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _aPISettings = options.Value;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _config = config;
            _logger = logger;
        }

        public class RegisterUserRequest
        {
            public string Username { get; set; }
            public int Age { get; set; }
            public string Email { get; set; }
        }
        public class RegisterUserResponse
        {
            public Guid UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }
        public static class InMemoryUserStore
        {
            public static List<RegisterUserResponse> Users = new();
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            // Preconditions (duplicated for demo, though oracle also checks)
            if (request.Age < 18)
                return BadRequest("User must be at least 18 years old.");

            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                return BadRequest("Invalid email address.");

            var newUser = new RegisterUserResponse
            {
                UserId = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email
            };

            InMemoryUserStore.Users.Add(newUser);

            return CreatedAtAction(nameof(GetById), new { id = newUser.UserId }, new
            {
                message = "User registered successfully",
                userId = newUser.UserId,
                username = newUser.Username,
                email = newUser.Email
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user = InMemoryUserStore.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();
            return Ok(user);
        }





        [HttpPost]
        //[AllowAnonymous]
        //[Authorize(Policy = Permissions.AddUser)]
        public async Task<IActionResult> SignUp([FromBody] UserRequestDTO userRequestDTO)
        {
            if (userRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = new XcelAppUser
            {
                UserName = userRequestDTO.Email,
                Email = userRequestDTO.Email,
                Name = userRequestDTO.Name,
                PhoneNumber = userRequestDTO.PhoneNo,
                EmailConfirmed = true,
                ProviderIds = userRequestDTO.ProviderIds,
                AccountType = userRequestDTO.AccountType,
            };

            var result = await _userManager.CreateAsync(user, userRequestDTO.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterationResponseDTO
                { Errors = errors, IsRegisterationSuccessful = false });
            }
            //var roleResult = await _userManager.AddToRoleAsync(user, SD.Role_Customer);
            //if (!roleResult.Succeeded)
            //{
            //    var errors = result.Errors.Select(e => e.Description);
            //    return BadRequest(new RegisterationResponseDTO
            //    { Errors = errors, IsRegisterationSuccessful = false });
            //}

            // Assume you add Permissions.All = List of all constants
            var validPermissions = Permissions.All;
            if (userRequestDTO.Permissions.Any(p => !validPermissions.Contains(p)))
            {
                return BadRequest("Invalid permissions selected.");
            }

            // Assign selected permissions as claims
            foreach (var permission in userRequestDTO.Permissions)
            {
                await _userManager.AddClaimAsync(user, new Claim("Permission", permission));
            }

            // Generate an email confirmation token
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            //var appUrl = $"{_config.GetValue<string>("AppUrl")}";
            //var confirmationLink = $"{appUrl}/api/xcelaccount/confirmemail?userId={user.Id}&token={token}";
            // Send the confirmation link via email
            //SendConfirmationEmail(user.Email, confirmationLink);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var appUrl = $"{_config.GetValue<string>("UIUrl")}";
            var resetUrl = $"{appUrl}/resetpassword?email={user.Email}&token={token}";
            SendNewAccountPasswordResetEmail(user.Email, resetUrl);

            //if (_userManager.Options.SignIn.RequireConfirmedAccount)
            //return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "User Created Successfully", });
            return StatusCode(201); //record created successfully
        }
        void SendConfirmationEmail(string email, string confirmationLink)
        {
            var subject = "Confirm Your Email Address";
            var body = $"<p>Please confirm your email address created on the Xcel Claims submission system by clicking the link below:</p><p><a href=\"{confirmationLink}\">Confirm Email</a></p>";
            var emailRequest = new EmailRequest { Body = body, Subject = subject, To = email };
            _emailService.SendEmailAsync(emailRequest);
        }

        void SendNewAccountPasswordResetEmail(string email, string resetLink)
        {
            var subject = "Set Account Password";
            var body = $@"
                        <p>Dear user,</p>
                        <p>Your email address has been used to create an account on the Xcel Claims submission system.</p>
                        <p>To confirm your email address, please set a password by clicking the link below:</p>
                        <p><a href=""{resetLink}"">Confirm Email</a></p>
                        <br>
                        <p>This is an auto-generated email, please do not reply.</p>";
            var emailRequest = new EmailRequest { Body = body, Subject = subject, To = email };
            _emailService.SendEmailAsync(emailRequest);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] AuthenticationDTO authenticationDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(authenticationDTO.UserName,
                authenticationDTO.Password, false, false);

            if (result.IsLockedOut)
            {
                return Unauthorized(new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "This account is disabled."
                });
            }

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(authenticationDTO.UserName);
                if (user == null)
                {
                    return Unauthorized(new AuthenticationResponseDTO
                    {
                        IsAuthSuccessful = false,
                        ErrorMessage = "Invalid Authentication"
                    });
                }
                //everything is valid and we need to login the user
                var signinCredentials = GetSigningCredentials();
                var claims = await GetClaims(user);

                // Manually set the audiences in the payload
                var jwtPayload = new JwtPayload(
                    _aPISettings.ValidIssuer,
                    null, // Audience is handled separately
                    claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddHours(24)
                );

                jwtPayload["aud"] = _aPISettings.ValidAudience; // Add multiple audiences as an array

                var jwtHeader = new JwtHeader(signinCredentials);
                var jwtToken = new JwtSecurityToken(jwtHeader, jwtPayload);

                var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                return Ok(new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = true,
                    Token = token,
                    userDTO = new UserDTO
                    {
                        Name = user.Name,
                        Id = user.Id,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        ProviderIds = user.ProviderIds,
                        InsurerIds = await _unitOfWork.XcelHspSite.GetInsurerListByProviderIds(user.ProviderIds),
                        AccountType = user.AccountType,
                        AccountStatus = user.LockoutEnd.HasValue && user.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow ? "Inactive" : "Active"
                        //UserID = user.UserID
                    }
                });
            }
            else
            {
                return Unauthorized(new AuthenticationResponseDTO
                {
                    IsAuthSuccessful = false,
                    ErrorMessage = "Invalid Authentication"
                });
            }
        }
        private SigningCredentials GetSigningCredentials()
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_aPISettings.SecretKey));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private async Task<List<Claim>> GetClaims(XcelAppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Email),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id),

            };

            var roles = await _userManager.GetRolesAsync(user);  //await _userManager.FindByEmailAsync(user.Email)
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                claims.Add(claim);
            }

            return claims;
        }
        
        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            try
            {
                var users = await _userManager.Users
                .ToListAsync();

                var userDtos = new List<UserDTO>();
                foreach (var user in users)
                {
                    var userDto = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        AccountType = user.AccountType,
                        ProviderIds = user.ProviderIds,
                        AccountStatus = user.LockoutEnd.HasValue && user.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow ? "Inactive" : "Active"
                    };
                    userDtos.Add(userDto);
                }
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, null, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserListByProviderId([FromQuery] string providerIds)
        {
            try
            {
                List<int> providerList = providerIds.Split(',')
                                            .Select(int.Parse)
                                            .ToList();

                // Get users and filter based on providerIds
                var users = await _userManager.Users
                    .Where(user => user.ProviderIds != null &&
                                   providerList.Any(pid => user.ProviderIds.Contains(pid.ToString())))
                    .ToListAsync();

                var userDtos = new List<UserDTO>();
                foreach (var user in users)
                {
                    var userDto = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        AccountType = user.AccountType,
                        ProviderIds = user.ProviderIds,
                        AccountStatus = user.LockoutEnd.HasValue && user.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow ? "Inactive" : "Active"
                    };
                    userDtos.Add(userDto);
                }

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, null, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }


        [HttpGet]
        public async Task<IActionResult> UserInfo(string Id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user == null)
                {
                    return NotFound();
                }
                var claims = await _userManager.GetClaimsAsync(user);
                var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
                return Ok(new UserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ProviderIds = user.ProviderIds,
                    AccountType = user.AccountType,
                    Permissions = permissions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, Id, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserDTO model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning("Invalid input data: {@model}", model);
                return BadRequest(new ActionExecutionResponse { ActionMessage = "Invalid input data" });
            }
            //var user = await _userManager.GetUserAsync(User);
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return BadRequest(new ActionExecutionResponse());
                //return Unauthorized();
            }

            user.Name = model.Name ?? user.Name;
            //user.Email = model.Email ?? user.Email;           //exclude email update
            user.PhoneNumber = model.PhoneNo ?? user.PhoneNumber;
            user.ProviderIds = model.ProviderIds ?? user.ProviderIds;
            user.AccountType = model.AccountType ?? user.AccountType;

            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ActionExecutionResponse { ActionMessage = "Failed to update user information." });
                }
                // 🛑 Update Permissions
                if (model.Permissions != null)
                {
                    var currentClaims = await _userManager.GetClaimsAsync(user);

                    // Remove existing permission claims
                    var permissionClaims = currentClaims.Where(c => c.Type == "Permission").ToList();
                    foreach (var claim in permissionClaims)
                    {
                        await _userManager.RemoveClaimAsync(user, claim);
                    }
                    // Add new permissions
                    foreach (var permission in model.Permissions)
                    {
                        await _userManager.AddClaimAsync(user, new Claim("Permission", permission));
                    }
                }

                _logger.LogInformation("User {Email} info updated successfully.", user.Email);
                return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "User information updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, user.Email, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse { ActionMessage = "Failed to update user information." });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new ActionExecutionResponse { ActionMessage = "User ID or token is missing." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new ActionExecutionResponse { ActionMessage = "User not found." });
            }

            if (user.EmailConfirmed)
            {
                return BadRequest(new ActionExecutionResponse { ActionMessage = "Invalid Token" });
            }

            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Email confirmed successfully." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, userId, User.Identity?.Name);
            }
            
            return BadRequest(new ActionExecutionResponse { ActionMessage = "Email confirmation failed." });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmEmailDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new ActionExecutionResponse { ActionMessage = "User not found." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var appUrl = $"{_config.GetValue<string>("UIUrl")}";
            var resetUrl = $"{appUrl}/resetpassword?email={user.Email}&token={token}";

            try
            {
                // Send the confirmation link via email
                //SendConfirmationEmail(user.Email, confirmationLink);
                SendNewAccountPasswordResetEmail(user.Email, resetUrl);
                return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Email confirmation token sent to email." });
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur while sending the email
                _logger.LogError(ex, SD.LogErrorMsg, model, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse { ActionMessage = "Failed to send email confirmation token."} );
            }

            //return Ok(new { Message = "Password reset token sent." });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new ActionExecutionResponse { ActionMessage = "User not found." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var appUrl = $"{_config.GetValue<string>("UIUrl")}";
            var resetUrl = $"{appUrl}/resetpassword?email={user.Email}&token={token}";

            // Send the reset URL to the user's email address
            var subject = "Password Reset Request";
            var body = $"<p>Dear user, you have requested to reset your password on the Xcel Claims submission system. Please click the link below to reset your password:</p><p><a href=\"{resetUrl}\">Reset Password</a></p>";
            var emailRequest = new EmailRequest { Body = body, Subject = subject, To = user.Email };
            try
            {
                _emailService.SendEmailAsync(emailRequest);
                return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Password reset token sent to email." });
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur while sending the email
                _logger.LogError(ex, SD.LogErrorMsg, model, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse { ActionMessage = "Failed to send password reset email." });
            }

            //return Ok(new { Message = "Password reset token sent." });
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new ActionExecutionResponse { ActionMessage = "User not found." });
            }

            try
            {
                string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Password reset successfully." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, model, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
            
            return BadRequest(new ActionExecutionResponse { ActionMessage = "Password reset failed." });
        }

        [HttpPost]
        // Ensure that the user is authenticated
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();  // User not found
            }

            // Ensure that the new password and confirm password match
            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest(new ActionExecutionResponse { ActionMessage = "New password and confirmation do not match." });
            }

            // Change the user's password
            try
            {
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "Password changed successfully." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, model, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }

            return BadRequest(new ActionExecutionResponse { ActionMessage = "Password change failed" });

            // If the password change fails, return the errors
            //var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            //return BadRequest(new { Message = "Password change failed", Errors = errors });
        }

        [HttpGet]
        public IActionResult GetAllPermissions()
        {

            return Ok(Permissions.All);
        }
        [HttpPatch]
        public async Task<IActionResult> DisableUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return NotFound(new ActionExecutionResponse { ActionMessage = "User not found." });

                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

                return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "User has been disabled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, userId, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
            
        }
        [HttpPatch]
        public async Task<IActionResult> EnableUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return NotFound(new ActionExecutionResponse { ActionMessage = "User not found." });

                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);

                return Ok(new ActionExecutionResponse { IsActionSuccessful = true, ActionMessage = "User has been re-enabled." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, SD.LogErrorMsg, userId, User.Identity?.Name);
                return BadRequest(new ActionExecutionResponse());
            }
            
        }

    }
}
