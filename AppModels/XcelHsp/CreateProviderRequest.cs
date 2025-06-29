using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.XcelHsp
{
    public class CreateProviderRequest
    {
        public string ServiceProvider { get; set; }
        public string InsurerIds { get; set; }
        public string GPSAddress { get; set; }
        public string ImageUrl { get; set; }
    }
}
