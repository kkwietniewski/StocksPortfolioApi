using Microsoft.Extensions.Options;
using StocksPortfolio.Api.ApiModels;
using StocksPortfolio.Infrastructure.Models;
using System.Text;
using System.Text.Json;

namespace StocksPortfolio.Core.Services
{
    public class ExchangeRateProvderService
    {
        // Docs: https://currencylayer.com/documentation

        private readonly IOptions<CurrencyLayerModel> _currencyLayer;
        private readonly HttpClient _httpClient;
        private readonly HttpResponseMessage _currencies;

        public QuoteModel QuoteModel { get; set; }

        public ExchangeRateProvderService(IOptions<CurrencyLayerModel> currencyLayer)
        {
            _currencyLayer = currencyLayer;
            _httpClient = new HttpClient { BaseAddress = new Uri(currencyLayer.Value.Url) };
            _currencies = _httpClient.GetAsync($"live?access_key={_currencyLayer.Value.ApiKey}").Result;
        }

        public void ScheduleRefreshCurrenciesCollection()
        {
            Timer timer = new Timer(_ => RefreshCurrencies(), null, TimeSpan.Zero, TimeSpan.FromHours(24));
        }

        private void RefreshCurrencies()
        {
            Task.Run(GetCurrencies).GetAwaiter().GetResult();
        }

        public async Task<Stream> GetCurrenciesStream()
        {
            var response = await _currencies.Content.ReadAsStringAsync();
            byte[] byteArray = Encoding.UTF8.GetBytes(response);
            return new MemoryStream(byteArray);
        }

        public async Task GetCurrencies()
        {
            using Stream stream = await GetCurrenciesStream() ?? throw new Exception("Cannot load data from currencylayer.com");
            QuoteModel = await JsonSerializer.DeserializeAsync<QuoteModel>(stream);
        }
    }
}
