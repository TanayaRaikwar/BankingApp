using Newtonsoft.Json;
using Plain.RabbitMQ;
using Shared.Models;
using Transaction.API.Database;

namespace Transaction.API
{
    public class AccountResponseListener : IHostedService
    {
        private ISubscriber _subscriber;
        private readonly IServiceScopeFactory _scopeFactory;

        public AccountResponseListener(ISubscriber subscriber, IServiceScopeFactory scopeFactory)
        {
            _subscriber = subscriber;
            _scopeFactory = scopeFactory;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(Subscribe);
            return Task.CompletedTask;
        }

        private bool Subscribe(string message, IDictionary<string, object> header)
        {
            var response = JsonConvert.DeserializeObject<AccountResponse>(message);
            if (!response.IsSuccess)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _transactionContext = scope.ServiceProvider.GetService<TransactionContext>();

                    // if transaction is not successful, remove transaction item
                    var transaction = _transactionContext.Transactions.Where(o => o.AccountId == response.AccountId && o.TransactionId == response.TransactionId).FirstOrDefault();
                    

                    _transactionContext.Transactions.Remove(transaction);
                    _transactionContext.SaveChanges();
                }
            }
            return true;
        }

    }
}