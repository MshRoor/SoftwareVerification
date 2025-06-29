using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Account
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string InsurerIds { get; set; }
        public string ProviderIds { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }
        //public int UserID { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
