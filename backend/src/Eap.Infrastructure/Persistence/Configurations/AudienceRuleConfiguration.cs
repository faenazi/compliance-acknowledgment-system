using Eap.Domain.Audience;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class AudienceRuleConfiguration
    : IEntityTypeConfiguration<AudienceRule>
{
    public void Configure(EntityTypeBuilder<AudienceRule> builder)
    {
        builder.ToTable("AudienceRules", "acknowledgment");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.AudienceDefinitionId).IsRequired();

        builder.Property(r => r.RuleType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.RuleValue).HasMaxLength(256);
        builder.Property(r => r.IsExclusion).IsRequired();
        builder.Property(r => r.SortOrder).IsRequired();

        builder.Property(r => r.CreatedAtUtc).IsRequired();
        builder.Property(r => r.UpdatedAtUtc);
        builder.Property(r => r.CreatedBy).HasMaxLength(256);
        builder.Property(r => r.UpdatedBy).HasMaxLength(256);

        builder.HasIndex(r => new { r.AudienceDefinitionId, r.IsExclusion, r.SortOrder });
    }
}
