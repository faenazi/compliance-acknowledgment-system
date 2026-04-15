using Eap.Application.Identity.Abstractions;
using Eap.Application.Identity.Services;
using Eap.Infrastructure.Identity;
using Eap.Infrastructure.Identity.Ldap;
using Eap.Infrastructure.Identity.Persistence;
using Eap.Infrastructure.Identity.Seeding;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eap.Infrastructure;

/// <summary>
/// Registers Infrastructure-layer services: EF Core, persistence, and
/// integration clients (LDAP in Sprint 1, Exchange in Sprint 8).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddIdentity(services, configuration);
        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EapDatabase");

        services.AddDbContext<EapDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
        });
    }

    private static void AddIdentity(IServiceCollection services, IConfiguration configuration)
    {
        // LDAP options — values come from configuration/environment/secrets.
        services.AddOptions<LdapOptions>()
            .Bind(configuration.GetSection(LdapOptions.SectionName))
            .ValidateDataAnnotations();

        services.AddOptions<UserProvisioningOptions>()
            .Bind(configuration.GetSection(UserProvisioningOptions.SectionName));

        services.AddOptions<IdentitySeedOptions>()
            .Bind(configuration.GetSection(IdentitySeedOptions.SectionName));

        // LDAP
        services.AddSingleton<LdapConnectionFactory>();
        services.AddScoped<LdapUserDirectory>();
        services.AddScoped<IUserDirectory>(sp => sp.GetRequiredService<LdapUserDirectory>());
        services.AddScoped<ILdapAuthenticator, LdapAuthenticationService>();

        // Persistence adapters
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDefaultRoleAssigner, DefaultRoleAssigner>();

        // Audit sink
        services.AddScoped<IIdentityAuditLogger, IdentityAuditLogger>();

        // Seeder
        services.AddScoped<IdentitySeeder>();
    }
}
