using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace FappLogger;

public static class Configure
{
    public static WebApplicationBuilder ConfigureWebApi(this WebApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        
        services.AddControllers();
        ConfigureSwagger(services);
        
        return builder;
    }
    
    private static void ConfigureSwagger(IServiceCollection services)
    {
        #region Swagger

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "FappLog", Version = "v1.0" });
            
            // Allow swagger to handle JWT Bearer tokens and authorization
            // Should not change "oauth2" to another name
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            // That is a need for authorization in swagger to
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        services.Configure<SwaggerUIOptions>(options =>
        {
            options.DocExpansion(DocExpansion.None); // Set default collapse state
        });
        
        #endregion
    }
}