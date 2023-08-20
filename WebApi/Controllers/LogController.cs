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
    private readonly KafkaLogProducerService _kafkaProducer;

    public LogController(IApplicationDbContext context, KafkaLogProducerService kafkaProducer)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
    }

    [HttpPost]
    public async Task CreateLog(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("LogController.CreateLog");
        Log log = new Log
        {
            Template = "Hello {Name}",
            Level = LogLevel.Information,
            Timestamp = DateTime.Now
        };
        await _context.Logs.InsertOneAsync(log, cancellationToken: cancellationToken);
    }
    
    [HttpPost("log")]
    public async Task Log([FromBody]Log log, CancellationToken cancellationToken = default)
    {
        await _context.Logs.InsertOneAsync(log, cancellationToken: cancellationToken);
    }
    
    [HttpPost("log-message")]
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
        
        // _kafkaProducer.ProduceMessage(kafkaLogMessage);
        // return Task.CompletedTask;
    }
}