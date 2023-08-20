using Application.Common.Interfaces;
using FappCommon.Kafka.Config;
using FappCommon.Mongo4Test;
using FappCommon.Mongo4Test.Implementations;
using Infrastructure.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Configure
{
    public const string CorsPolicy = "CorsPolicy"; // The name of the cors policy.
    
    public static WebApplicationBuilder ConfigureInfrastructure(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;

        #region DbContext

        MongoDbOptions options = new()
        {
            ConnectionStringName = "LogMongoDb",
            DatabaseName = "Logs"
        };
        services.AddMongoDbContext<IApplicationDbContext, ApplicationDbContext>(options);
        // BaseMongoDbContext.RunMigrations<ApplicationDbContext>(options, builder.Configuration);
        
        #endregion

        #region Kafka

        services.AddSingleton<KafkaConsumerConfig>();
        services.AddSingleton<KafkaProducerConfig>(); // TODO: Remove

        #endregion
        
        return builder;
    }
    
    private static void ConfigureCors(IServiceCollection services)
    {
        services
            .AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                    bld =>
                    {
                        bld.WithOrigins("*") // TODO: Change in prod to the api gateway address
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin()
                            .WithExposedHeaders("Content-Disposition");
                    });
            });
    }
}