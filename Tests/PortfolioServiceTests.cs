using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Moq;
using StocksPortfolio.Core.Services;
using StocksPortfolio.Infrastructure.Entities;
using StocksPortfolio.Infrastructure.Interfaces;
using StocksPortfolio.Infrastructure.Models;

namespace Tests
{
    public class PortfolioServiceTests
    {
        private readonly CurrencyLayerModel _currencyLayerModel;
        private readonly DataProviderService _dataProviderService;
        private readonly Mock<IOptions<CurrencyLayerModel>> _configMock;
        private readonly Mock<ExchangeRateProvderService> _exchangeRateProvderServiceMock;
        private readonly Mock<StocksService.StocksService> _stocksServiceMock;

        public PortfolioServiceTests()
        {
            _currencyLayerModel = new CurrencyLayerModel { ApiKey = "78c057e28b2abf54f48110356bb9d1ce", Url = "http://api.currencylayer.com/" };
            _dataProviderService = new DataProviderService();
            _configMock = new Mock<IOptions<CurrencyLayerModel>>();
            _configMock.Setup(x => x.Value).Returns(_currencyLayerModel);
            _exchangeRateProvderServiceMock = new Mock<ExchangeRateProvderService>(_configMock.Object);
            _stocksServiceMock = new Mock<StocksService.StocksService>();
        }

        [Fact]
        public void GetPortfolio_ValidId_ReturnsPortfolio()
        {
            // Arrange
            var portfolioId = "50227b375dff9218248eadc4";
            var portfolioService = new PortfolioService(_dataProviderService, _exchangeRateProvderServiceMock.Object, _stocksServiceMock.Object);

            // Act
            var portfolio = portfolioService.GetPortfolio(portfolioId);

            // Assert
            Assert.NotNull(portfolio);
            Assert.Equal(portfolioId, portfolio.Id.ToString());
        }

        [Fact]
        public void GetAllPortfolios_NotEmpty_ReturnsTrueFalse()
        {
            // Arrange
            var portfolioService = new PortfolioService(_dataProviderService, _exchangeRateProvderServiceMock.Object, _stocksServiceMock.Object);

            // Act
            var portfolios = portfolioService.GetAllPortfolios();

            // Assert
            Assert.NotEmpty(portfolios);
        }

        [Fact]
        public void GetAllPortfolios_IncludesExpectedIds_ReturnsEmptyExpectedList()
        {
            // Arrange
            var portfolioService = new PortfolioService(_dataProviderService, _exchangeRateProvderServiceMock.Object, _stocksServiceMock.Object);
            var expectedIds = new List<ObjectId>
            {
                ObjectId.Parse("50227b375dff9218248eadc4"),
                ObjectId.Parse("50227b375dff9218248eadc5"),
                ObjectId.Parse("50227b375dff9218248eadc6")
            };

            // Act
            var portfolios = portfolioService.GetAllPortfolios();

            // Assert
            Assert.All(portfolios,portfolio =>
            {
                Assert.Contains(portfolio.Id, expectedIds);
                expectedIds.Remove(portfolio.Id);
            });

            Assert.Empty(expectedIds);
        }

        [Fact]
        public void GetPortfolio_InvalidId_ThrowsException()
        {
            // Arrange
            var portfolioId = "50227b375dff9218248eadc7"; //Incorrect id
            var portfolioService = new PortfolioService(_dataProviderService, _exchangeRateProvderServiceMock.Object, _stocksServiceMock.Object);

            // Act & Assert
            Assert.Throws<Exception>(() => portfolioService.GetPortfolio(portfolioId));
        }

        [Fact]
        public async void DeletePortfolio_DeleteElement_NotReturnsDeletedElement()
        {
            // Arrange
            var portfolioId = "50227b375dff9218248eadc6";
            var portfolioService = new PortfolioService(_dataProviderService, _exchangeRateProvderServiceMock.Object, _stocksServiceMock.Object);

            // Act & Assert
            await portfolioService.DeletePortfolio(portfolioId);
            Assert.Throws<Exception>(() => portfolioService.GetPortfolio(portfolioId));
        }
    }
}