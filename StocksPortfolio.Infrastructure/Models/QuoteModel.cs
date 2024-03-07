using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace StocksPortfolio.Api.ApiModels
{
    public class QuoteModel
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("terms")]
        public string? Terms { get; set; }
        [JsonPropertyName("privacy")]
        public string? Privacy { get; set; }
        [JsonPropertyName("timestamp")]
        public int Timestamp { get; set; }
        [JsonPropertyName("source")]
        public string? Source { get; set; }
        [JsonPropertyName("quotes")]
        public Dictionary<string, decimal>? Quotes { get; set; }
    }
}
