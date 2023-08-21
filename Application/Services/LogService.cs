using Application.Common.Interfaces;
using Domain.Entities.LogEntities;
using FappCommon.Kafka.Log;
using MongoDB.Bson;

namespace Application.Services;

public class LogService : IDisposable
{
    private readonly IApplicationDbContext _context;

    public LogService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(KafkaLogMessage message, CancellationToken cancellationToken = default)
    {
        Log log = new Log
        {
            Template = message.Template,
            Level = message.LogLevel,
            Timestamp = message.TimespanAsUtc,
            Data = BsonDocument.Parse(message.Data)
        };
        
        await _context.Logs.InsertOneAsync(log, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}