using Application.Common.Interfaces;
using Application.Services;
using Domain.Entities.LogEntities;
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
    private readonly KafkaProducer _kafkaProducer;

    public LogController(IApplicationDbContext context, KafkaProducer kafkaProducer)
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
    public async Task Log([FromBody]string message, CancellationToken cancellationToken = default)
    {
       _kafkaProducer.ProduceMessage(message);
    }
}