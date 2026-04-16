using Eap.Domain.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class UserSubmissionFieldValueConfiguration
    : IEntityTypeConfiguration<UserSubmissionFieldValue>
{
    public void Configure(EntityTypeBuilder<UserSubmissionFieldValue> builder)
    {
        builder.ToTable("UserSubmissionFieldValues", "acknowledgment");

        builder.HasKey(fv => fv.Id);

        builder.Property(fv => fv.UserSubmissionId).IsRequired();

        builder.Property(fv => fv.FieldKey)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(fv => fv.FieldLabel)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(fv => fv.FieldType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(fv => fv.ValueText).HasMaxLength(4000);
        builder.Property(fv => fv.ValueNumber).HasColumnType("decimal(18,6)");
        builder.Property(fv => fv.ValueDate);
        builder.Property(fv => fv.ValueBoolean);
        builder.Property(fv => fv.ValueJson).HasColumnType("nvarchar(max)");

        builder.Property(fv => fv.CreatedAtUtc).IsRequired();
        builder.Property(fv => fv.UpdatedAtUtc);

        builder.HasIndex(fv => new { fv.UserSubmissionId, fv.FieldKey });
    }
}
