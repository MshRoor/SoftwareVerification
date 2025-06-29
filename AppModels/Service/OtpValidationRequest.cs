using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Service
{
    public class OtpValidationRequest
    {
        public string MembershipNumber { get; set; }
        public string OTP { get; set; }
    }
}
