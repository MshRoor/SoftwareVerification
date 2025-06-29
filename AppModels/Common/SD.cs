using AppModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppModels.Common
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";
        public const string Role_Employee = "Employee";

        //public const string Local_InitialBooking = "InitialRoomBookingInfo";
        //public const string Local_RoomOrderDetails = "RoomOrderDetails";
        //public const string Local_Token = "JWT Token";
        //public const string Local_UserDetails = "User Details";

        public static string JWTToken { get; set; } = "";
        public static UserDTO UserData { get; set; } = new();

        //public const string Status_Pending = "Pending";
        //public const string Status_Booked = "Booked";
        //public const string Status_CheckedIn = "CheckedIn";
        //public const string Status_CheckedOut_Completed = "CheckedOut";
        //public const string Status_NoShow = "NoShow";
        //public const string Status_Cancelled = "Cancelled";

        public const string Invalid_SP_ReturnValue = "Unknown Action";

        public const string Prescription_FilePath = "Files/Prescriptions";

        public const string LogErrorMsg = "An error occurred while performing the operation on {@data} by User {user}";

        public const string HangFireQueueName = "softwareverification";
    }
}
