using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; } = null!;
}