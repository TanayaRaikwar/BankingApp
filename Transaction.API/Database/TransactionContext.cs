using Microsoft.EntityFrameworkCore;
using Transaction.API.Models;

namespace Transaction.API.Database
{
    public class TransactionContext : DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options) : base(options)
        {

        }

        public DbSet<AmountTransaction> Transactions { get; set; }
    }
}