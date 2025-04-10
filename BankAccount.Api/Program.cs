using Amazon.SimpleNotificationService;
using BankAccount.Infrastructure.Data;
using BankAccount.Infrastructure.Repositories;
using BankAccount.Core.Interfaces;
using BankAccount.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<BankDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// AWS
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
object value = builder.Services.AddAWSService<AmazonSimpleNotificationServiceClient>();
builder.Services.AddSingleton(sp=> {
    return builder.Configuration["AWS:WithdrawalEventsTopicArn"];
});

// Application Services
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

// Initialize database


// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();