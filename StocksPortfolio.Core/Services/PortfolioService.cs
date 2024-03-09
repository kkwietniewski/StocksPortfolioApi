using MongoDB.Bson;
using StocksPortfolio.Api.ApiModels;
using StocksPortfolio.Infrastructure.Entities;

namespace StocksPortfolio.Core.Services
{
    public partial class PortfolioService
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

        public Portfolio GetPortfolio(string id) 
            => _dataService.GetPortfolio(ObjectId.Parse(id)).Result ?? throw new Exception($"Portfolio with id {id} not found.");

        public List<Portfolio> GetAllPortfolios()
        {
            var portfolios = _dataService.GetAllPortfolios();
            if (portfolios == null || !portfolios.Any())
                throw new Exception($"Cannot find any portfolios.");
            return portfolios;
        }

        public decimal GetTotalAmout(string portfolioId, string currency)
        {
            var portfolio = _dataService.GetPortfolio(ObjectId.Parse(portfolioId)).Result;
            var totalAmount = 0m;
            var data = _exchangeRateProvderService.QuoteModel ?? throw new Exception($"Cannot get any quote.");
            if (data.Quotes is null) throw new Exception($"Cannot get any quote.");

            foreach (var stock in portfolio.Stocks)
            {
                totalAmount += CalculateStockAmount(currency, stock, data);
            }

            return totalAmount;
        }

        public async Task DeletePortfolio(string id)
        {
            GetPortfolio(id);
            await _dataService.DeletePortfolio(ObjectId.Parse(id));
        }

        private decimal CalculateStockAmount(string currency, Stock stock, QuoteModel data)
        {
            var amount = 0m;
            var externalStockPrice = _stocksService.GetStockPrice(stock.Ticker).Result.Price;
            var externalStockCurrency = _stocksService.GetStockPrice(stock.Ticker).Result.Currency;
            var externalRateUsd = externalStockCurrency == "USD" ? 1 : data.Quotes["USD" + externalStockCurrency];

            if (stock.Currency == currency)
            {
                if (externalStockCurrency == currency)
                    amount += externalStockPrice * stock.NumberOfShares;
                else
                    amount += externalStockPrice / externalRateUsd * stock.NumberOfShares;
            }
            else
            {
                var rateUsd = stock.Currency == "USD" ? 1 : data.Quotes["USD" + stock.Currency];
                if (currency == "USD")
                {
                    if (externalStockCurrency == currency)
                        amount += externalStockPrice / rateUsd * stock.NumberOfShares;
                    else
                        amount += externalStockPrice * externalRateUsd * stock.NumberOfShares;
                }
                else
                {
                    if (externalStockCurrency == currency)
                        amount += externalStockPrice / rateUsd * stock.NumberOfShares;
                    else
                    {
                        var externalPriceUsd = externalStockPrice / externalRateUsd;
                        var targetRateUsd = data.Quotes["USD" + currency];
                        if (stock.Currency == externalStockCurrency)
                            amount += externalPriceUsd * targetRateUsd * stock.NumberOfShares;
                        else
                            amount += externalPriceUsd / rateUsd * targetRateUsd * stock.NumberOfShares;
                    }
                }
            }

            return amount;
        }
    }
}
