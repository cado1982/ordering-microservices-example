using Accounting.AsyncDataServices;
using Accounting.Data;
using Accounting.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQL Server");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("AccountingMSSqlConnection")));
}
else
{
   Console.WriteLine("--> Using In Memory Database");
   builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("AccountingInMem"));
}

builder.Services.AddControllers();

builder.Services.AddScoped<IAccountingRepository, AccountingRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddSwaggerGen();

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
app.MapGrpcService<GrpcAccountingService>();
app.MapGet("/protos/accounts.proto", async context => {
    await context.Response.WriteAsync(File.ReadAllText("Protos/accounts.proto"));
});

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

app.Run();
