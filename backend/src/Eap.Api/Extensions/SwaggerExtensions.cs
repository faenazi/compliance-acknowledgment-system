using Microsoft.OpenApi.Models;

namespace Eap.Api.Extensions;

/// <summary>
/// Swagger / OpenAPI configuration for the EAP API.
/// </summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddEapSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Enterprise Acknowledgment Platform API",
                Version = "v1",
                Description = "Internal API for the Enterprise Acknowledgment Platform (EAP).",
                Contact = new OpenApiContact
                {
                    Name = "The Environment Fund"
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseEapSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "EAP API v1");
            options.DocumentTitle = "EAP API";
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}
