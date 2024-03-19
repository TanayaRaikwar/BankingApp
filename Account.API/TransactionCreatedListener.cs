using Account.API.Database;
using Account.API.Models;
using Newtonsoft.Json;
using Plain.RabbitMQ;
using Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using Shared.Enums;

namespace Account.API
{
    public class TransactionCreatedListener : IHostedService
    {
        private ISubscriber _subscriber;
        private IPublisher _publisher;

        private readonly IServiceScopeFactory _scopeFactory;

        public TransactionCreatedListener(IServiceScopeFactory scopeFactory, ISubscriber subscriber, IPublisher publisher)
        {
            _publisher = publisher;
            _subscriber = subscriber;
            _scopeFactory = scopeFactory;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<TransactionRequest>(message);
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<AccountContext>();
                try
                {
                    BankAccount BankAccount = _context.Accounts.Find(response.AccountId);
                    // if available balance is less then requested amount
                    if (BankAccount == null || BankAccount.Balance < response.Amount)
                        throw new Exception();


                    //  reduce the amount if debit
                    BankAccount.Balance = BankAccount.Balance - response.Amount;
                    _context.Entry(BankAccount).State = EntityState.Modified;
                    _context.SaveChanges();


                    // BankAccount bankAccount = _context.Accounts.Find(response.AccountId);
                    // if (bankAccount == null)
                    //     throw new Exception("Account not found");

                    // // Handle credit transaction
                    // if (response.TransactionType == TransactionType.Credit)
                    // {
                    //     bankAccount.Balance += response.Amount;
                    // }
                    // // Handle debit transaction
                    // else if (response.TransactionType == TransactionType.Debit)
                    // {
                    //     if (bankAccount.Balance < response.Amount)
                    //         throw new Exception("Insufficient funds");

                    //     bankAccount.Balance -= response.Amount;
                    // }
                    // else
                    // {
                    //     throw new Exception("Invalid transaction type");
                    // }

                    // _context.Entry(bankAccount).State = EntityState.Modified;
                    // _context.SaveChanges();



                    // publish message to inform Transaction service for success
                    _publisher.Publish(JsonConvert.SerializeObject(
                        new AccountResponse
                        {
                            TransactionId = response.TransactionId,
                            AccountId = response.AccountId,
                            Balance = response.Balance,
                            IsSuccess = true
                        }
                    ), "account_response_routingkey", null);
                }
                catch (Exception ex)
                {
                    // publish message to inform Transaction service for failed
                    _publisher.Publish(JsonConvert.SerializeObject(
                        new AccountResponse
                        {
                            TransactionId = response.TransactionId,
                            AccountId = response.AccountId,
                            Balance = response.Balance,
                            IsSuccess = false
                        }
                    ), "account_response_routingkey", null);
                }
                return true;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}