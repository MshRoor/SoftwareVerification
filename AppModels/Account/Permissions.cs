using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Account
{
    public static class Permissions
    {
        public const string AddUser = "Permissions.AddUser";
        public const string CreateClaim = "Permissions.CreateClaim";
        public const string SubmitClaim = "Permissions.SubmitClaim";
        public const string ReopenClaim = "Permissions.ReopenClaim";
        public const string PreAuthorization = "Permissions.PreAuthorization";
        public const string CloseClaim = "Permissions.CloseClaim";
        public const string AccountManagement = "Permissions.AccountManagement";

        public static List<string> All => new()
        {
            AddUser,CreateClaim, SubmitClaim, ReopenClaim, PreAuthorization, CloseClaim, AccountManagement
            //{ "User Management", new List<string> { AddUser, AccountManagement } },
            //{ "Claims Management", new List<string> { CreateClaim, SubmitClaim, ReopenClaim, PreAuthorization, CloseClaim } }
        };
    }
}
