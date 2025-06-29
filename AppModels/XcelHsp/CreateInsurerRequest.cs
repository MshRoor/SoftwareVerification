using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.XcelHsp
{
    public class CreateInsurerRequest
    {
        public string CompanyName { get; set; }
        public string UIColor { get; set; }
        public string APIUrl { get; set; }
        public string LogoUrl { get; set; }
    }
}
