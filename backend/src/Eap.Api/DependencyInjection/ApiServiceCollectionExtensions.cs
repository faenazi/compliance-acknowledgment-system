using Microsoft.OpenApi.Models;

namespace Eap.Api.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddHealthChecks();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "EAP API",
                Version = "v1",
                Description = "Enterprise Acknowledgment Platform backend foundation"
            });
        });

        return services;
    }
}
