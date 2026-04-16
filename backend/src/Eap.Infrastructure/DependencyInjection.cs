using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Admin.Abstractions;
using Eap.Application.Audience.Abstractions;
using Eap.Application.Forms.Abstractions;
using Eap.Application.Identity.Abstractions;
using Eap.Application.Identity.Services;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Requirements.Abstractions;
using Eap.Application.Requirements.Services;
using Eap.Application.UserPortal.Abstractions;
using Eap.Infrastructure.Acknowledgments.Audit;
using Eap.Infrastructure.Acknowledgments.Persistence;
using Eap.Infrastructure.Admin.Persistence;
using Eap.Infrastructure.Audience.Audit;
using Eap.Infrastructure.Audience.Resolution;
using Eap.Infrastructure.Forms.Audit;
using Eap.Infrastructure.Forms.Persistence;
using Eap.Infrastructure.Forms.Storage;
using Eap.Infrastructure.Identity;
using Eap.Infrastructure.Identity.Ldap;
using Eap.Infrastructure.Identity.Persistence;
using Eap.Infrastructure.Identity.Seeding;
using Eap.Infrastructure.Persistence;
using Eap.Infrastructure.Policies.Audit;
using Eap.Infrastructure.Policies.Documents;
using Eap.Infrastructure.Policies.Persistence;
using Eap.Infrastructure.Requirements.Audit;
using Eap.Infrastructure.Requirements.Persistence;
using Eap.Infrastructure.UserPortal.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        AddPolicyManagement(services, configuration);
        AddAcknowledgmentManagement(services, configuration);
        AddAudienceTargeting(services, configuration);
        AddRequirementGeneration(services, configuration);
        AddFormDisclosures(services, configuration);
        AddUserPortal(services);
        AddAdminPortal(services);
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

    private static void AddPolicyManagement(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PolicyDocumentStorageOptions>()
            .Bind(configuration.GetSection(PolicyDocumentStorageOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddSingleton<IPolicyDocumentStorage, FileSystemPolicyDocumentStorage>();
        services.AddScoped<IPolicyAuditLogger, PolicyAuditLogger>();
    }

    private static void AddAcknowledgmentManagement(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAcknowledgmentRepository, AcknowledgmentRepository>();
        services.AddScoped<IAcknowledgmentAuditLogger, AcknowledgmentAuditLogger>();
    }

    private static void AddAudienceTargeting(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAudienceResolver, AudienceResolver>();
        services.AddScoped<IDirectoryGroupResolver, StubDirectoryGroupResolver>();
        services.AddScoped<IAudienceAuditLogger, AudienceAuditLogger>();
    }

    private static void AddFormDisclosures(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<FormUploadStorageOptions>()
            .Bind(configuration.GetSection(FormUploadStorageOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IFormDefinitionRepository, FormDefinitionRepository>();
        services.AddScoped<IUserSubmissionRepository, UserSubmissionRepository>();
        services.AddSingleton<IFormUploadStorage, FileSystemFormUploadStorage>();
        services.AddScoped<IFormAuditLogger, FormAuditLogger>();
    }

    private static void AddUserPortal(IServiceCollection services)
    {
        services.AddScoped<IUserPortalRepository, UserPortalRepository>();
    }

    private static void AddAdminPortal(IServiceCollection services)
    {
        services.AddScoped<IAdminRepository, AdminRepository>();
    }

    private static void AddRequirementGeneration(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRequirementRepository, RequirementRepository>();
        services.AddScoped<IRequirementAuditLogger, RequirementAuditLogger>();
        services.AddScoped<IRequirementGenerator, RequirementGenerator>();

        // TimeProvider powers deterministic cycle-key derivation; System default is
        // adequate — tests swap in a FakeTimeProvider.
        services.TryAddSingleton(TimeProvider.System);
    }
}
