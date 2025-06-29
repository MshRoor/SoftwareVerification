using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels
{
    public class ActionExecutionResponse
    {
        public int ReturnValue { get; set; }
        public string? ReturnData { get; set; }
        public bool IsActionSuccessful { get; set; } = false;
        public string ActionMessage { get; set; } = "An error occurred while performing the operation!";
    }
}
