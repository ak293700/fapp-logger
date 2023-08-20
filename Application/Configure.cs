using Application.Services;
using FappCommon.Kafka.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Configure
{
    public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        
        #region Services

        services.AddScoped<KafkaLogProducerService>();
        services.AddHostedService<KafkaLogConsumerService>();

        #endregion
        
        return builder;
    }
}