using Eap.Domain.Policy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies", "policy");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PolicyCode)
            .IsRequired()
            .HasMaxLength(64);
        builder.HasIndex(p => p.PolicyCode).IsUnique();

        builder.Property(p => p.Title).IsRequired().HasMaxLength(256);
        builder.Property(p => p.OwnerDepartment).IsRequired().HasMaxLength(256);
        builder.Property(p => p.Category).HasMaxLength(128);
        builder.Property(p => p.Description).HasMaxLength(4000);

        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.CurrentPolicyVersionId);

        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc);
        builder.Property(p => p.CreatedBy).HasMaxLength(256);
        builder.Property(p => p.UpdatedBy).HasMaxLength(256);

        builder.HasMany(p => p.Versions)
            .WithOne()
            .HasForeignKey(v => v.PolicyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Policy.Versions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
