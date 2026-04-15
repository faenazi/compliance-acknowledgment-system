using Eap.Domain.Acknowledgment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class AcknowledgmentDefinitionConfiguration
    : IEntityTypeConfiguration<AcknowledgmentDefinition>
{
    public void Configure(EntityTypeBuilder<AcknowledgmentDefinition> builder)
    {
        builder.ToTable("AcknowledgmentDefinitions", "acknowledgment");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title).IsRequired().HasMaxLength(256);
        builder.Property(d => d.OwnerDepartment).IsRequired().HasMaxLength(256);
        builder.Property(d => d.Description).HasMaxLength(4000);

        builder.Property(d => d.DefaultActionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(d => d.CurrentAcknowledgmentVersionId);

        builder.Property(d => d.CreatedAtUtc).IsRequired();
        builder.Property(d => d.UpdatedAtUtc);
        builder.Property(d => d.CreatedBy).HasMaxLength(256);
        builder.Property(d => d.UpdatedBy).HasMaxLength(256);

        builder.HasMany(d => d.Versions)
            .WithOne()
            .HasForeignKey(v => v.AcknowledgmentDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(AcknowledgmentDefinition.Versions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
