using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace StocksPortfolio.Infrastructure.Entities
{
    public class Portfolio
    {
        [BsonElement("id")]
        public ObjectId Id { get; set; }

        [BsonElement("totalValue")]
        public float CurrentTotalValue { get; set; }

        [BsonElement("deleted")]
        public bool Deleted { get; set; }

        [BsonElement("stocks")]
        public ICollection<Stock> Stocks { get; set; }
    }
}
