using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Persistence;

/// <summary>
/// Primary EF Core DbContext for the EAP platform.
/// Entity sets are introduced per module in later sprints (Sprint 1 onwards).
/// </summary>
public class EapDbContext : DbContext
{
    public EapDbContext(DbContextOptions<EapDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Module-specific IEntityTypeConfiguration<T> registrations are added in later sprints via:
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(EapDbContext).Assembly);
    }
}
