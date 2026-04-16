using Eap.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", "notification");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.RecipientEmail).IsRequired().HasMaxLength(320);
        builder.Property(n => n.RelatedEntityType).IsRequired().HasMaxLength(128);
        builder.Property(n => n.Subject).IsRequired().HasMaxLength(512);
        builder.Property(n => n.BodyHtml).IsRequired().HasColumnType("nvarchar(max)");

        builder.Property(n => n.NotificationType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(n => n.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(n => n.UserId).HasDatabaseName("IX_Notifications_UserId");
        builder.HasIndex(n => n.Status).HasDatabaseName("IX_Notifications_Status");
        builder.HasIndex(n => new { n.RelatedEntityType, n.RelatedEntityId })
            .HasDatabaseName("IX_Notifications_RelatedEntity");

        // Sprint 9: composite index for notification dedup check (ExistsAsync).
        builder.HasIndex(n => new { n.UserId, n.NotificationType, n.RelatedEntityId, n.Status })
            .HasDatabaseName("IX_Notifications_Dedup");

        builder.HasMany(n => n.Attempts)
            .WithOne()
            .HasForeignKey(a => a.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Map CreatedAtUtc / UpdatedAtUtc from Entity base.
        builder.Property(n => n.CreatedAtUtc);
        builder.Property(n => n.UpdatedAtUtc);
    }
}
