using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using StocksPortfolio.Core.Services;

namespace StocksPortfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly DataProviderService _dataService;
        private readonly PortfolioService _portfolioService;

        public PortfolioController(DataProviderService dataService, PortfolioService portfolioService)
        {
            _dataService = dataService;
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var portfolios = _dataService.GetAllPortfolios().Result;
            return Ok(portfolios);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var portfolio = _dataService.GetPortfolio(ObjectId.Parse(id)).Result;
            return Ok(portfolio);
        }

        [HttpGet("value/{portfolioId}/{currency}")]
        public IActionResult GetTotalPortfolioValue(string portfolioId, string currency = "USD")
        {
            var totalAmount = _portfolioService.GetTotalAmout(portfolioId, currency);
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
