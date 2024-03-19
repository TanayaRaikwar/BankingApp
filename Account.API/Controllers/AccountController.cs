

using Account.API.Database;
using Account.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Account.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountContext _context;

        public AccountController(AccountContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task PostBankAccount(BankAccount bankAccount)
        {
            _context.Accounts.Add(bankAccount);
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
        {
            var bankAccount = await _context.Accounts.FindAsync(id);
            if(bankAccount == null)
                return NotFound();
            return bankAccount;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount(int id, BankAccount bankAccount)
        {
            if (id != bankAccount.Id)
            {
                return BadRequest();
            }

            _context.Entry(bankAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount(int id)
        {
            var bankAccount = await _context.Accounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(bankAccount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankAccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }


    }
}