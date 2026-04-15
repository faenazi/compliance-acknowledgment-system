using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eap.Infrastructure;

/// <summary>
/// Registers Infrastructure-layer services: EF Core, persistence, and
/// integration clients (LDAP / Exchange — wired in later sprints).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EapDatabase");

        services.AddDbContext<EapDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
        });

        // LDAP / Active Directory and Exchange clients are registered in Sprint 1 and Sprint 8 respectively.

        return services;
    }
}
