using Microsoft.AspNetCore.Builder;

namespace Application;

public static class Configure
{
    public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder builder)
    {
        return builder;
    }
}