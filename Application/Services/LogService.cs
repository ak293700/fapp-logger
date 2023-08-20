using Application.Common.Interfaces;
using FappCommon.Kafka.Log;

namespace Application.Services;

public class LogService
{
    private readonly IApplicationDbContext _context;

    public LogService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Log(KafkaLogMessage message)
    {
        // await _context.Logs.InsertOneAsync(message);
    }
}