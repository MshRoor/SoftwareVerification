using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Account
{
    public class RegisterationResponseDTO
    {
        public bool IsRegisterationSuccessful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
