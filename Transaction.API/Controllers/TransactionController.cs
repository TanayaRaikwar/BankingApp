using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Transaction.API.Database;
using Transaction.API.Models;
using Plain.RabbitMQ;
using Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Transaction.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionContext _context;
        private readonly IPublisher _publisher;

        public TransactionController(TransactionContext context, IPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AmountTransaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AmountTransaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return NotFound();
            return transaction;
        }

        [HttpPost]
        public async Task PostTransaction(AmountTransaction amountTransaction)
        {
            _context.Transactions.Add(amountTransaction);
            await _context.SaveChangesAsync();

            // new inserted identity value
            int id = amountTransaction.Id;

            _publisher.Publish(JsonConvert.SerializeObject(new TransactionRequest
            {
                TransactionId = amountTransaction.TransactionId,
                AccountId = amountTransaction.AccountId,
                Amount = amountTransaction.Amount,
            }),
              "transaction_created_routingkey",   // routing key
            null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, AmountTransaction amountTransaction)
        {
            if (id != amountTransaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(amountTransaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }



    }
}