using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;
using StocksPortfolio.Api.ApiModels;
using StocksPortfolio.Core.Services;

namespace StocksPortfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly DataProviderService _dataService;
        private readonly StocksService.StocksService _stocksService;

        public PortfolioController(DataProviderService dataService, StocksService.StocksService stocksService)
        {
            _dataService = dataService;
            _stocksService = stocksService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var portfolio = _dataService.GetPortfolio(ObjectId.Parse(id)).Result;
            return Ok(portfolio);
        }

        [HttpGet("value/{portfolioId}/{currency}")]
        public IActionResult GetTotalPortfolioValue(string portfolioId, string currency = "USD")
        {
            var portfolio = _dataService.GetPortfolio(ObjectId.Parse(portfolioId)).Result;
            var totalAmount = 0m;
            var apiAccessKey = "78c057e28b2abf54f48110356bb9d1ce";
            using (var httpClient = new HttpClient { BaseAddress = new Uri("http://api.currencylayer.com/") })
            {
                // Docs: https://currencylayer.com/documentation
                var foo = httpClient.GetAsync($"live?access_key={apiAccessKey}").Result;
                var data = JsonSerializer.DeserializeAsync<QuoteModel>(foo.Content.ReadAsStream()).Result;
                if (data is null) throw new Exception("Cannot load data from currencylayer.com");
                data.Quotes ??= new Dictionary<string, decimal>();
                foreach (var stock in portfolio.Stocks)
                {
                    if (stock.Currency == currency)
                    {
                        totalAmount += _stocksService.GetStockPrice(stock.Ticker).Result.Price * stock.NumberOfShares;
                    }
                    else
                    {
                        var isRateUsd = data.Quotes.TryGetValue("USD" + stock.Currency, out var rateUsd);
                        if (currency == "USD")
                        {
                            var stockPrice = _stocksService.GetStockPrice(stock.Ticker).Result.Price;
                            if (isRateUsd)
                                totalAmount += stockPrice / rateUsd * stock.NumberOfShares;
                        }
                        else
                        {
                            var stockPrice = _stocksService.GetStockPrice(stock.Ticker).Result.Price;
                            if (isRateUsd)
                            {
                                var amount = stockPrice / rateUsd * stock.NumberOfShares;
                                var targetRateUsd = data.Quotes["USD" + currency];
                                totalAmount += amount * targetRateUsd;
                            }
                        }
                    }
                }
            }

            return Ok(totalAmount);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(string id)
        {
            await _dataService.DeletePortfolio(ObjectId.Parse(id));
            return Ok();
        }
    }
}
