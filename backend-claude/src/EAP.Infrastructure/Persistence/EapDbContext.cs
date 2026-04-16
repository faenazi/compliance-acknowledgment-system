using EAP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EAP.Infrastructure.Persistence;

public class EapDbContext : DbContext
{
    public EapDbContext(DbContextOptions<EapDbContext> options) : base(options)
    {
    }

    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyId);
            entity.Property(e => e.PolicyCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PolicyTitle).HasMaxLength(500).IsRequired();
            entity.Property(e => e.OwnerDepartment).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreatedBy).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId);
            entity.Property(e => e.ActionType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityType).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
        });
    }
}
