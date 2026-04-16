using Eap.Domain.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class UserSubmissionConfiguration
    : IEntityTypeConfiguration<UserSubmission>
{
    public void Configure(EntityTypeBuilder<UserSubmission> builder)
    {
        builder.ToTable("UserSubmissions", "acknowledgment");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.AcknowledgmentVersionId).IsRequired();
        builder.Property(s => s.FormDefinitionId).IsRequired();

        builder.Property(s => s.SubmissionJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.FormDefinitionSnapshotJson)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.SubmittedAtUtc).IsRequired();

        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc);
        builder.Property(s => s.CreatedBy).HasMaxLength(256);
        builder.Property(s => s.UpdatedBy).HasMaxLength(256);

        builder.HasIndex(s => new { s.AcknowledgmentVersionId, s.UserId });
        builder.HasIndex(s => s.UserId);

        // Flattened field values owned by the submission.
        builder.HasMany(s => s.FieldValues)
            .WithOne()
            .HasForeignKey(fv => fv.UserSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(UserSubmission.FieldValues))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
