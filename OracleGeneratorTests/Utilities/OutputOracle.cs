using OracleGeneratorTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleGeneratorTests.Utilities
{
    public static class OutputOracle
    {
        public static bool Validate(ApiSpec spec, HttpResponseMessage response, string responseBody, out List<string> failures)
        {
            failures = new List<string>();

            if ((int)response.StatusCode != spec.Expected_Output.Status_Code)
                failures.Add($"Expected status {spec.Expected_Output.Status_Code}, got {(int)response.StatusCode}");

            if (!responseBody.Contains(spec.Expected_Output.Message))
                failures.Add("Expected message not found in response.");

            foreach (var key in spec.Expected_Output.Data_Keys)
            {
                if (!responseBody.Contains(key))
                    failures.Add($"Expected data key '{key}' not found.");
            }

            return !failures.Any();
        }
    }
}
