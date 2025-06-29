using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleGeneratorTests.Models
{
    public class InputSpec
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
    }

    public class Conditions
    {
        public string Condition { get; set; }
        public string Message { get; set; }
    }

    public class ExpectedOutput
    {
        public int Status_Code { get; set; }
        public string Message { get; set; }
        public List<string> Data_Keys { get; set; }
    }

    public class ApiSpec
    {
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public List<InputSpec> Inputs { get; set; }
        public List<Conditions> Preconditions { get; set; }
        public List<Conditions> Postconditions { get; set; }
        public ExpectedOutput Expected_Output { get; set; }
    }
}
