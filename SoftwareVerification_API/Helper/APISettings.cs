using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftwareVerification_API.Helper
{
    public class APISettings
    {
        public string SecretKey { get; set; }
        public List<string> ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
    }
}
