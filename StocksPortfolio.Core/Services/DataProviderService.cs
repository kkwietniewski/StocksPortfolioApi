using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using StocksPortfolio.Infrastructure.Entities;
using StocksPortfolio.Infrastructure.Interfaces;

namespace StocksPortfolio.Core.Services
{
    public class DataProviderService : IDataProvider
    {
        private readonly IMongoCollection<Portfolio> _portfolioCollection;
        private static readonly MongoDbRunner _runner = MongoDbRunner.Start();

        static DataProviderService()
        {
            _runner.Import("portfolioDb", "Portfolios", Path.Combine("Data", "portfolios.json"), true);
        }

        public DataProviderService()
        {
            var client = new MongoClient(_runner.ConnectionString);
            _portfolioCollection = client.GetDatabase("portfolioDb").GetCollection<Portfolio>("Portfolios");
        }

        public List<Portfolio> GetAllPortfolios()
            => _portfolioCollection.AsQueryable().Where(portfolio => portfolio.Deleted == false).ToList();

        public async Task<Portfolio> GetPortfolio(ObjectId id)
        {
            var idFilter = Builders<Portfolio>.Filter.Where(portfolio => portfolio.Id == id && portfolio.Deleted == false);

            return await _portfolioCollection.Find(idFilter).FirstOrDefaultAsync();
        }

        public async Task DeletePortfolio(ObjectId id)
        {
            var portfolio = GetPortfolio(id).Result;
            await _portfolioCollection.DeleteOneAsync(Builders<Portfolio>.Filter.Eq(portfolio => portfolio.Id, id));
            portfolio.Deleted = true;
            await _portfolioCollection.InsertOneAsync(portfolio);
        }
    }
}
