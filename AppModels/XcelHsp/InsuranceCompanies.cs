using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.XcelHsp
{
    public class InsuranceCompanies
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string UIColor { get; set; }
        public string APIUrl { get; set; }
        public string LogoUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
