using Domain.Entities.LogEntities;
using FappCommon.Mongo4Test.Interfaces;
using MongoDB.Driver;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext: IBaseMongoDbContext
{
    public IMongoCollection<Log> Logs { get; }
}