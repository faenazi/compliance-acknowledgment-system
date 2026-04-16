using Eap.Domain.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class FormDefinitionConfiguration
    : IEntityTypeConfiguration<FormDefinition>
{
    public void Configure(EntityTypeBuilder<FormDefinition> builder)
    {
        builder.ToTable("FormDefinitions", "acknowledgment");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.AcknowledgmentVersionId).IsRequired();

        builder.Property(f => f.SchemaVersion).IsRequired();
        builder.Property(f => f.IsActive).IsRequired();

        builder.Property(f => f.CreatedAtUtc).IsRequired();
        builder.Property(f => f.UpdatedAtUtc);
        builder.Property(f => f.CreatedBy).HasMaxLength(256);
        builder.Property(f => f.UpdatedBy).HasMaxLength(256);

        // One form definition per version.
        builder.HasIndex(f => f.AcknowledgmentVersionId).IsUnique();

        // Ordered field collection.
        builder.HasMany(f => f.Fields)
            .WithOne()
            .HasForeignKey(ff => ff.FormDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(FormDefinition.Fields))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Ignore computed properties.
        builder.Ignore(f => f.HasAnyInputField);
    }
}
