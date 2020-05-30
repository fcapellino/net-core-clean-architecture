namespace MainProject.Domain.Models
{
    using Common;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Document : IMongoEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRequired]
        public string Content { get; set; }

        [BsonRequired]
        public string ContractAddress { get; set; }

        [BsonRequired]
        public string DownloadLink { get; set; }

        [BsonRequired]
        public string EtherscanLink { get; set; }

        [BsonRequired]
        public string Guid { get; set; }

        [BsonRequired]
        public string Hash { get; set; }

        [BsonRequired]
        public string Name { get; set; }

        [BsonRequired]
        public string[] Tags { get; set; }

        [BsonRequired]
        public string TransactionHash { get; set; }

        [BsonRequired]
        public string Type { get; set; }

        [BsonRequired]
        public bool IsDeleted { get; set; }
    }
}
