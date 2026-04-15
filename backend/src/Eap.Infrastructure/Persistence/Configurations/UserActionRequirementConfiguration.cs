using Eap.Domain.Requirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class UserActionRequirementConfiguration
    : IEntityTypeConfiguration<UserActionRequirement>
{
    public void Configure(EntityTypeBuilder<UserActionRequirement> builder)
    {
        builder.ToTable("UserActionRequirements", "acknowledgment");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.AcknowledgmentVersionId).IsRequired();

        builder.Property(r => r.CycleReference)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(r => r.RecurrenceInstanceDate);
        builder.Property(r => r.DueDate);
        builder.Property(r => r.AssignedAtUtc).IsRequired();
        builder.Property(r => r.CompletedAtUtc);

        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.IsCurrent).IsRequired();

        builder.Property(r => r.CreatedAtUtc).IsRequired();
        builder.Property(r => r.UpdatedAtUtc);
        builder.Property(r => r.CreatedBy).HasMaxLength(256);
        builder.Property(r => r.UpdatedBy).HasMaxLength(256);

        // A cycle is unique per (user, version).
        builder.HasIndex(r => new { r.UserId, r.AcknowledgmentVersionId, r.CycleReference })
            .IsUnique()
            .HasDatabaseName("UX_UserActionRequirements_User_Version_Cycle");

        // Fast lookup for "my open requirements" queries.
        builder.HasIndex(r => new { r.UserId, r.Status, r.IsCurrent });

        // Fast lookup for "requirements for this version" queries.
        builder.HasIndex(r => new { r.AcknowledgmentVersionId, r.Status });
    }
}
