using Application.Common.Interfaces;
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

    public LogController(IApplicationDbContext context)
    {
        _context = context;
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
}