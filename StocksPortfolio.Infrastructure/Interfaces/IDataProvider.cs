using MongoDB.Bson;
using StocksPortfolio.Infrastructure.Entities;

namespace StocksPortfolio.Infrastructure.Interfaces
{
    public interface IDataProvider
    {
        Task<Portfolio> GetPortfolio(ObjectId id);
        List<Portfolio> GetAllPortfolios();
        Task DeletePortfolio(ObjectId id);
    }
}
