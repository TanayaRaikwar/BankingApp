using Shared.Enums;

namespace Shared.Models
{
    public class TransactionRequest
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public TransactionType TransactionType {get; set;}
    }
}