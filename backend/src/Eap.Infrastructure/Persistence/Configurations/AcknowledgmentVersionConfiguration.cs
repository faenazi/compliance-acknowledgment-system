using Eap.Domain.Acknowledgment;
using Eap.Domain.Audience;
using Eap.Domain.Forms;
using Eap.Domain.Policy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class AcknowledgmentVersionConfiguration
    : IEntityTypeConfiguration<AcknowledgmentVersion>
{
    public void Configure(EntityTypeBuilder<AcknowledgmentVersion> builder)
    {
        builder.ToTable("AcknowledgmentVersions", "acknowledgment");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.AcknowledgmentDefinitionId).IsRequired();
        builder.Property(v => v.VersionNumber).IsRequired();
        builder.Property(v => v.VersionLabel).HasMaxLength(128);
        builder.Property(v => v.Summary).HasMaxLength(4000);
        builder.Property(v => v.CommitmentText).HasMaxLength(4000);

        builder.Property(v => v.PolicyVersionId).IsRequired();

        builder.Property(v => v.ActionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.RecurrenceModel)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.StartDate);
        builder.Property(v => v.DueDate);
        builder.Property(v => v.PublishedAtUtc);
        builder.Property(v => v.PublishedBy).HasMaxLength(256);
        builder.Property(v => v.ArchivedAtUtc);
        builder.Property(v => v.ArchivedBy).HasMaxLength(256);
        builder.Property(v => v.SupersededByAcknowledgmentVersionId);

        builder.Property(v => v.CreatedAtUtc).IsRequired();
        builder.Property(v => v.UpdatedAtUtc);
        builder.Property(v => v.CreatedBy).HasMaxLength(256);
        builder.Property(v => v.UpdatedBy).HasMaxLength(256);

        // Monotonic version numbers per definition.
        builder.HasIndex(v => new { v.AcknowledgmentDefinitionId, v.VersionNumber }).IsUnique();

        // Defence-in-depth: at most one Published version per definition.
        builder.HasIndex(v => new { v.AcknowledgmentDefinitionId, v.Status })
            .IsUnique()
            .HasFilter($"[Status] = {(int)AcknowledgmentVersionStatus.Published}")
            .HasDatabaseName("UX_AcknowledgmentVersions_Definition_Published");

        // LR-001: read-only FK into the policy module. Restrict delete so the
        // policy-version row can never be removed while an ack version still
        // references it — preserves audit history.
        builder.HasOne<PolicyVersion>()
            .WithMany()
            .HasForeignKey(v => v.PolicyVersionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.PolicyVersionId);

        // 0..1 audience owned by the version (BR-050). Deleting the version
        // cascades to its audience configuration.
        builder.HasOne(v => v.Audience)
            .WithOne()
            .HasForeignKey<AudienceDefinition>(a => a.AcknowledgmentVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        // 0..1 form definition owned by the version (BR-070). Deleting the
        // version cascades to its form definition.
        builder.HasOne(v => v.FormDefinition)
            .WithOne()
            .HasForeignKey<FormDefinition>(f => f.AcknowledgmentVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
