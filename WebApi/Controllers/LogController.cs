using Application.Common.Interfaces;
using Domain.Entities.LogEntities;
using FappCommon.Kafka.Log;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FappLogger.Controllers;

[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
[ApiController]
public class LogController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public LogController(IApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public async Task Log([FromBody]KafkaLogMessage kafkaLogMessage, CancellationToken cancellationToken = default)
    {
        Log log = new Log
        {
            Template = kafkaLogMessage.Template,
            Level = kafkaLogMessage.LogLevel,
            Timestamp = DateTime.Now,
        };
        
        BsonDocument bsonDocument = BsonDocument.Parse(kafkaLogMessage.Data);
        log.Data = bsonDocument;
        
        await _context.Logs.InsertOneAsync(log, cancellationToken: cancellationToken);
    }
}