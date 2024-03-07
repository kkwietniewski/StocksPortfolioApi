using MongoDB.Bson;
using StocksPortfolio.Infrastructure.Entities;
using StocksService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksPortfolio.Core.Services
{
    public class PortfolioService
    {
        private readonly DataProviderService _dataService;
        private readonly ExchangeRateProvderService _exchangeRateProvderService;
        private readonly StocksService.StocksService _stocksService;

        public PortfolioService(DataProviderService dataService, ExchangeRateProvderService exchangeRateProvderService, StocksService.StocksService stocksService)
        {
            _dataService = dataService;
            _exchangeRateProvderService = exchangeRateProvderService;
            _stocksService = stocksService;
        }

        public decimal GetTotalAmout(string portfolioId, string currency)
        {
            var portfolio = _dataService.GetPortfolio(ObjectId.Parse(portfolioId)).Result;
            var totalAmount = 0m;
            var data = _exchangeRateProvderService.GetCurrencies().Result;
            data.Quotes ??= new Dictionary<string, decimal>();
            foreach (var stock in portfolio.Stocks)
            {
                var stockPrice = _stocksService.GetStockPrice(stock.Ticker).Result.Price;
                if (stock.Currency == currency)
                {
                    totalAmount += stockPrice * stock.NumberOfShares;
                }
                else
                {
                    if (currency == "USD")
                    {
                        var rateUsd = data.Quotes["USD" + stock.Currency];
                        totalAmount += stockPrice / rateUsd * stock.NumberOfShares;
                    }
                    else
                    {
                        var rateUsd = data.Quotes["USD" + stock.Currency];
                        var amount = stockPrice / rateUsd * stock.NumberOfShares;
                        var targetRateUsd = data.Quotes["USD" + currency];
                        totalAmount += amount * targetRateUsd;
                    }
                }
            }

            return totalAmount;
        }
    }
}
