using Eap.Domain.Policy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class PolicyVersionConfiguration : IEntityTypeConfiguration<PolicyVersion>
{
    public void Configure(EntityTypeBuilder<PolicyVersion> builder)
    {
        builder.ToTable("PolicyVersions", "policy");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.PolicyId).IsRequired();
        builder.Property(v => v.VersionNumber).IsRequired();
        builder.Property(v => v.VersionLabel).HasMaxLength(128);
        builder.Property(v => v.Summary).HasMaxLength(4000);

        builder.Property(v => v.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.EffectiveDate);
        builder.Property(v => v.PublishedAtUtc);
        builder.Property(v => v.PublishedBy).HasMaxLength(256);
        builder.Property(v => v.ArchivedAtUtc);
        builder.Property(v => v.ArchivedBy).HasMaxLength(256);
        builder.Property(v => v.SupersededByPolicyVersionId);

        builder.Property(v => v.CreatedAtUtc).IsRequired();
        builder.Property(v => v.UpdatedAtUtc);
        builder.Property(v => v.CreatedBy).HasMaxLength(256);
        builder.Property(v => v.UpdatedBy).HasMaxLength(256);

        // Monotonic version numbers per policy.
        builder.HasIndex(v => new { v.PolicyId, v.VersionNumber }).IsUnique();

        // BR-011: at most one Published version per policy, enforced at the database layer.
        builder.HasIndex(v => new { v.PolicyId, v.Status })
            .IsUnique()
            .HasFilter($"[Status] = {(int)PolicyVersionStatus.Published}")
            .HasDatabaseName("UX_PolicyVersions_Policy_Published");

        // Navigation to the single attached document (if any).
        builder.HasOne(v => v.Document)
            .WithOne()
            .HasForeignKey<PolicyDocument>(d => d.PolicyVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.Document)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
