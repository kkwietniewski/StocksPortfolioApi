using StocksPortfolio.Core.Services;
using StocksPortfolio.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<DataProviderService>();
builder.Services.AddTransient<ExchangeRateProvderService>();
builder.Services.AddTransient<StocksService.StocksService>();
builder.Services.AddTransient<PortfolioService>();
builder.Services.Configure<CurrencyLayerModel>(builder.Configuration.GetSection("CurrencyLayer"));

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
