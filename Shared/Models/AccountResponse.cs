namespace Shared.Models
{
    public class AccountResponse
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal Balance{ get; set; }
        public bool IsSuccess { get; set; }
    }
}