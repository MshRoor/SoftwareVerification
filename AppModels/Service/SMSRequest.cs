using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Service
{
    public class SMSRequest
    {
        public string username {  get; set; }
        public string password { get; set; }
        public string source { get; set; }
        public string destination {  get; set; }
        public string message { get; set; }
    }
}
