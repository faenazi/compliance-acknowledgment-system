using Eap.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "audit");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActorUsername).HasMaxLength(256);
        builder.Property(a => a.ActionType).IsRequired().HasMaxLength(128);
        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(128);
        builder.Property(a => a.Description).HasMaxLength(1024);

        // Snapshot columns stored as nvarchar(max) for flexibility.
        builder.Property(a => a.BeforeSnapshotJson).HasColumnType("nvarchar(max)");
        builder.Property(a => a.AfterSnapshotJson).HasColumnType("nvarchar(max)");

        // Indexes for audit explorer queries.
        builder.HasIndex(a => a.ActionTimestampUtc).HasDatabaseName("IX_AuditLogs_Timestamp");
        builder.HasIndex(a => a.EntityType).HasDatabaseName("IX_AuditLogs_EntityType");
        builder.HasIndex(a => a.ActionType).HasDatabaseName("IX_AuditLogs_ActionType");
        builder.HasIndex(a => a.ActorUserId).HasDatabaseName("IX_AuditLogs_ActorUserId");
    }
}
