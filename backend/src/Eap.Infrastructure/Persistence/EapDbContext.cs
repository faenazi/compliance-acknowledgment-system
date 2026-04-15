using Eap.Domain.Identity;
using Eap.Domain.Policy;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EapDbContext).Assembly);
    }
}
