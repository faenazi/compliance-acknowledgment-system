using Eap.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "identity");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(256);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.Department).HasMaxLength(256);
        builder.Property(u => u.JobTitle).HasMaxLength(256);
        builder.Property(u => u.DirectoryReference).HasMaxLength(512);
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.LastSyncedAtUtc).IsRequired();
        builder.Property(u => u.CreatedAtUtc).IsRequired();

        builder.HasMany(u => u.RoleAssignments)
            .WithOne()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Read-only nav backed by a private list; ensure EF writes via the field.
        builder.Metadata
            .FindNavigation(nameof(User.RoleAssignments))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
