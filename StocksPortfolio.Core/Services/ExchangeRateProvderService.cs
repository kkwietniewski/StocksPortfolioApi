using Microsoft.Extensions.Options;
using StocksPortfolio.Api.ApiModels;
using StocksPortfolio.Infrastructure.Models;
using System.Text.Json;

namespace StocksPortfolio.Core.Services
{
    public class ExchangeRateProvderService
    {
        // Docs: https://currencylayer.com/documentation

        private readonly IOptions<CurrencyLayerModel> _currencyLayer;
        private readonly HttpClient _httpClient;
        private readonly HttpResponseMessage _currencies;

        public ExchangeRateProvderService(IOptions<CurrencyLayerModel> currencyLayer)
        {
            _currencyLayer = currencyLayer;
            _httpClient = new HttpClient { BaseAddress = new Uri(currencyLayer.Value.Url) };
            _currencies = _httpClient.GetAsync($"live?access_key={_currencyLayer.Value.ApiKey}").Result;
        }

        public async Task<Stream> GetCurrenciesStream()
            => await _currencies.Content.ReadAsStreamAsync();

        public async Task<QuoteModel> GetCurrencies()
        {
            using (Stream stream = await GetCurrenciesStream() ?? throw new Exception("Cannot load data from currencylayer.com"))
            {
                return await JsonSerializer.DeserializeAsync<QuoteModel>(stream);
            }
        }
    }
}
