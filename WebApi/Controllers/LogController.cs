using Application.Common.Interfaces;
using Application.Services;
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
    private readonly LogService _logService;

    public LogController(LogService logService)
    {
        _logService = logService;
    }
    
    [HttpPost]
    public async Task Log([FromBody]KafkaLogMessage kafkaLogMessage, CancellationToken cancellationToken = default)
    {
        await _logService.Create(kafkaLogMessage, cancellationToken);
    }
}