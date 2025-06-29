using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SoftwareVerification_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        public static List<object> Transactions = new();

        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] TransferRequest request)
        {
            if (request.from_account == request.to_account)
                return BadRequest("Accounts must be different.");

            if (request.amount <= 0)
                return BadRequest("Transfer amount must be greater than zero.");

            var transaction = new
            {
                TransactionId = Guid.NewGuid(),
                FromAccount = request.from_account,
                ToAccount = request.to_account,
                Amount = request.amount
            };

            Transactions.Add(transaction);

            return Ok(new
            {
                message = "Transfer completed successfully",
                transactionId = transaction.TransactionId,
                from_account = request.from_account,
                to_account = request.to_account,
                amount = request.amount
            });
        }
    }

    public class TransferRequest
    {
        public string from_account { get; set; }
        public string to_account { get; set; }
        public decimal amount { get; set; }
    }
}
