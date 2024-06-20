
using PaymentGateway.Api;
using PaymentGateway.Domain.Interfaces;
using PaymentGateway.Domain.Services;
using PaymentGateway.Infrastructure.Repositories;


const string BANK_API_BASE_ADDRESS = "http://localhost:8080/payments";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateInputAttribute>();
});
builder.Services.AddScoped<ValidateInputAttribute>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPaymentsRepository,PaymentsRepository>();

builder.Services.AddScoped<IPaymentsService, PaymentsService>();
builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddHttpClient("Bank_Client", c =>
{
    c.BaseAddress = new Uri(BANK_API_BASE_ADDRESS);
});

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
