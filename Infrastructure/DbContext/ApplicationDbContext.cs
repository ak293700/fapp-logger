using Application.Common.Interfaces;
using Domain.Entities.LogEntities;
using FappCommon.Mongo4Test.Implementations;
using MongoDB.Driver;

namespace Infrastructure.DbContext;

public class ApplicationDbContext : BaseMongoDbContext, IApplicationDbContext
{
    public IMongoCollection<Log> Logs { get; private set; } = null!;


    protected override void InitializeCollections(IMongoDatabase database)
    {
        Logs = database.GetCollection<Log>(nameof(Logs));
    }
}