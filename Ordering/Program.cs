using Microsoft.EntityFrameworkCore;
using Ordering.AsyncDataServices;
using Ordering.Data;
using Ordering.EventProcessing;
using Ordering.SyncDataService.Grpc;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQL Server");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("OrderingMSSqlConnection")));
}
else
{
    Console.WriteLine("--> Using In Memory Database");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("OrderingInMem"));
}

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<IAccountsDataClient, AccountsDataClient>();
builder.Services.AddHostedService<MessageBusBackgroundService>();
builder.Services.AddScoped<IMessageBusSubscriber, MessageBusSubscriber>();

builder.Services.AddScoped<IEventHandler, AccountPublishedEventHandler>();

var connectionFactory = new ConnectionFactory()
{
    HostName = builder.Configuration["RabbitMQHost"],
    Port = Int32.Parse(builder.Configuration["RabbitMQPort"])
};

builder.Services.AddSingleton<IConnectionFactory>(connectionFactory);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
