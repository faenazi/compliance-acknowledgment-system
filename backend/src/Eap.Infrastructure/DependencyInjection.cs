using Eap.Application.Abstractions;
using Eap.Infrastructure.DependencyInjection;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eap.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

        var options = configuration
            .GetSection(InfrastructureOptions.SectionName)
            .Get<InfrastructureOptions>() ?? new InfrastructureOptions();

        services.AddDbContext<AppDbContext>(dbOptions =>
        {
            dbOptions.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            });

            if (options.EnableSensitiveDataLogging)
            {
                dbOptions.EnableSensitiveDataLogging();
            }
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }
}
