using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Domain.Entities.LogEntities;

public class Log : BaseEntity
{
    public string Template { get; set; } = null!;
    public LogLevel Level { get; set; }
    public DateTime Timestamp { get; set; }

    public BsonDocument Data { get; set; } = null!;
}