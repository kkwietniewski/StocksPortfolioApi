using MongoDB.Bson.Serialization.Attributes;

namespace StocksPortfolio.Infrastructure.Entities
{
    public class Stock
    {
        [BsonElement("ticker")]
        public string Ticker { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; }

        [BsonElement("numberOfShares")]
        public int NumberOfShares { get; set; }
    }
}
