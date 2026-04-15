using Eap.Domain.Acknowledgment;
using Eap.Domain.Audience;
using Eap.Domain.Identity;
using Eap.Domain.Policy;
using Eap.Domain.Requirements;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Persistence;

/// <summary>
/// Primary EF Core DbContext for the EAP platform. Entity sets are introduced
/// per module. Module-specific <c>IEntityTypeConfiguration&lt;T&gt;</c> types
/// are discovered by scanning the Infrastructure assembly.
/// </summary>
public class EapDbContext : DbContext
{
    public EapDbContext(DbContextOptions<EapDbContext> options) : base(options)
    {
    }

    // Identity (Sprint 1)
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Scope> Scopes => Set<Scope>();
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();

    // Policy management (Sprint 2)
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<PolicyVersion> PolicyVersions => Set<PolicyVersion>();
    public DbSet<PolicyDocument> PolicyDocuments => Set<PolicyDocument>();

    // Acknowledgment management (Sprint 3)
    public DbSet<AcknowledgmentDefinition> AcknowledgmentDefinitions => Set<AcknowledgmentDefinition>();
    public DbSet<AcknowledgmentVersion> AcknowledgmentVersions => Set<AcknowledgmentVersion>();

    // Audience targeting & requirements (Sprint 4)
    public DbSet<AudienceDefinition> AudienceDefinitions => Set<AudienceDefinition>();
    public DbSet<AudienceRule> AudienceRules => Set<AudienceRule>();
    public DbSet<UserActionRequirement> UserActionRequirements => Set<UserActionRequirement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EapDbContext).Assembly);
    }
}
