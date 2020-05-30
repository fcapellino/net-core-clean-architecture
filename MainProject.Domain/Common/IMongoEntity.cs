namespace MainProject.Domain.Common
{
    using MongoDB.Bson;

    public interface IMongoEntity
    {
        ObjectId Id { get; set; }
        bool IsDeleted { get; set; }
    }
}
