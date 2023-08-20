using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.LogEntities;

public class Log : BaseEntity
{
    public string Template { get; set; } = null!;
    public LogLevel Level { get; set; }
    public DateTime Timestamp { get; set; }

    [BsonExtraElements] 
    public BsonDocument Data { get; set; } = null!;
}