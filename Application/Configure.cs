using Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static class Configure
{
    public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        
        #region Services

        services.AddScoped<KafkaProducer>();
        services.AddHostedService<KafkaConsumerService>();

        #endregion
        
        return builder;
    }
}