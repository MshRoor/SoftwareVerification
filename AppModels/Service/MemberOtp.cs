using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Service
{
    public class MemberOtp
    {
        public int Id { get; set; }
        public string MembershipNo { get; set; }
        public string OTP {  get; set; }
        public DateTime Expiration { get; set; }
    }
}
