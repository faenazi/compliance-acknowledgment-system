using Eap.Domain.Policy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class PolicyDocumentConfiguration : IEntityTypeConfiguration<PolicyDocument>
{
    public void Configure(EntityTypeBuilder<PolicyDocument> builder)
    {
        builder.ToTable("PolicyDocuments", "policy");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.PolicyVersionId).IsRequired();
        builder.Property(d => d.FileName).IsRequired().HasMaxLength(512);
        builder.Property(d => d.ContentType).IsRequired().HasMaxLength(128);
        builder.Property(d => d.FileSize).IsRequired();
        builder.Property(d => d.StorageReference).IsRequired().HasMaxLength(1024);
        builder.Property(d => d.UploadedAtUtc).IsRequired();

        builder.Property(d => d.CreatedAtUtc).IsRequired();
        builder.Property(d => d.UpdatedAtUtc);
        builder.Property(d => d.CreatedBy).HasMaxLength(256);
        builder.Property(d => d.UpdatedBy).HasMaxLength(256);

        // One document per version in MVP (FR-013). The FK is already unique via
        // the one-to-one relationship declared on PolicyVersion.
    }
}
