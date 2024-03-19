using Microsoft.EntityFrameworkCore;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using Transaction.API;
using Transaction.API.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TransactionContext>(options => 
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// configure rabbitmq
builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));

// configure rabbitmq exchange where order service publsih message
builder.Services.AddSingleton<IPublisher>(p => new Publisher(p.GetService<IConnectionProvider>(),
"transaction_exchange",   // exchange name
ExchangeType.Topic));   // exchange type

// configure topic where order service will be subscribe
builder.Services.AddSingleton<ISubscriber>(s => new Subscriber(
    s.GetService<IConnectionProvider>(),
    "account_exchange",         // exchange name
    "account_response_queue",   // queue name
    "account_response_routingkey", // routing key
    ExchangeType.Topic
));

// register the listener
builder.Services.AddHostedService<AccountResponseListener>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
