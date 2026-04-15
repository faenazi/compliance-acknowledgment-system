using Eap.Domain.Audience;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class AudienceDefinitionConfiguration
    : IEntityTypeConfiguration<AudienceDefinition>
{
    public void Configure(EntityTypeBuilder<AudienceDefinition> builder)
    {
        builder.ToTable("AudienceDefinitions", "acknowledgment");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AcknowledgmentVersionId).IsRequired();

        builder.Property(a => a.AudienceType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.CreatedAtUtc).IsRequired();
        builder.Property(a => a.UpdatedAtUtc);
        builder.Property(a => a.CreatedBy).HasMaxLength(256);
        builder.Property(a => a.UpdatedBy).HasMaxLength(256);

        // One audience per version.
        builder.HasIndex(a => a.AcknowledgmentVersionId).IsUnique();

        // Ordered rule collection — EF Core owns the backing list on the aggregate.
        builder.HasMany(a => a.Rules)
            .WithOne()
            .HasForeignKey(r => r.AudienceDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(AudienceDefinition.Rules))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore derived projections — they're computed on the fly.
        builder.Ignore(a => a.InclusionRules);
        builder.Ignore(a => a.ExclusionRules);
        builder.Ignore(a => a.HasAnyInclusionRule);
    }
}
