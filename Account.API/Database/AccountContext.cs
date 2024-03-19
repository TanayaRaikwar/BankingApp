using Account.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Account.API.Database
{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options) :base(options)
        {

        }

        public DbSet<BankAccount> Accounts {get; set;}
    }
}