using System.Text.Json;
using Eap.Domain.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class FormFieldConfiguration
    : IEntityTypeConfiguration<FormField>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public void Configure(EntityTypeBuilder<FormField> builder)
    {
        builder.ToTable("FormFields", "acknowledgment");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FormDefinitionId).IsRequired();

        builder.Property(f => f.FieldKey)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(f => f.Label)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.FieldType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(f => f.IsRequired).IsRequired();
        builder.Property(f => f.SortOrder).IsRequired();

        builder.Property(f => f.SectionKey).HasMaxLength(128);
        builder.Property(f => f.HelpText).HasMaxLength(1000);
        builder.Property(f => f.Placeholder).HasMaxLength(500);
        builder.Property(f => f.DisplayText).HasMaxLength(4000);

        // Options stored as JSON column.
        builder.Property(f => f.Options)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<IReadOnlyList<FieldOption>>(v, JsonOptions) ?? Array.Empty<FieldOption>())
            .HasColumnType("nvarchar(max)");

        builder.Property(f => f.CreatedBy).HasMaxLength(256);
        builder.Property(f => f.CreatedAtUtc).IsRequired();
        builder.Property(f => f.UpdatedAtUtc);

        builder.HasIndex(f => new { f.FormDefinitionId, f.SortOrder });
    }
}
