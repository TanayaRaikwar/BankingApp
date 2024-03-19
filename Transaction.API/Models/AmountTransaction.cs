using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Transaction.API.Enums;

namespace Transaction.API.Models
{
    public class AmountTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public TransactionType TransactionType{ get; set; }
        public decimal Amount { get; set; }
    }
}