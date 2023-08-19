using Microsoft.AspNetCore.Builder;
    
namespace Infrastructure;

public static class Configure
{
    public static WebApplicationBuilder ConfigureInfrastructure(this WebApplicationBuilder builder)
    {
        return builder;
    }
}