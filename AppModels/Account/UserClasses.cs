using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Account
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
    public class ResendConfirmEmailDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; } = "";

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Confirm new password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }

    public class UpdateUserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        [Required]
        public string ProviderIds { get; set; }
        [Required]
        public string AccountType { get; set; }
        public List<string> Permissions { get; set; } = new();
    }

}
