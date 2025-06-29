using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Account
{
    public class UserRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string PhoneNo { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password is not matched")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Service Provider is required")]
        public string ProviderIds { get; set; }

        [Required(ErrorMessage = "AccountType is required")]
        public string AccountType { get; set; }

        public List<string> Permissions { get; set; } = new();
    }
}
