using Account.API;
using Account.API.Database;
using Microsoft.EntityFrameworkCore;
using Plain.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// configure database service
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AccountContext>(options => 
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// configure rabbitmq
builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@localhost:5672"));

// register publisher 
builder.Services.AddSingleton<IPublisher>(p => new Publisher(p.GetService<IConnectionProvider>(),
    "account_exchange",
    ExchangeType.Topic
));

// register subscriber
builder.Services.AddSingleton<ISubscriber>(s => new Subscriber(s.GetService<IConnectionProvider>(),
"transaction_exchange",
"transaction_response_queue",
"transaction_created_routingkey",
ExchangeType.Topic
));

// register listener
builder.Services.AddHostedService<TransactionCreatedListener>();


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
