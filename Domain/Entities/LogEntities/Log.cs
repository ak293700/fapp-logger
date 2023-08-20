using Microsoft.Extensions.Logging;

namespace Domain.Entities.LogEntities;

public class Log : BaseEntity
{
    public string Template { get; set; } = null!;
    public LogLevel Level { get; set; }
    public DateTime Timestamp { get; set; }
}
