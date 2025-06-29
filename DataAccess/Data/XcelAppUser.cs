using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class XcelAppUser : IdentityUser
    {
        public string Name { get; set; }
        public string ProviderIds { get; set; }
        public string AccountType { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int UserID { get; set; }
    }
}
