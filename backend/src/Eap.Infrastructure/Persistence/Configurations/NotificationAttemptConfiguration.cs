using Eap.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class NotificationAttemptConfiguration : IEntityTypeConfiguration<NotificationAttempt>
{
    public void Configure(EntityTypeBuilder<NotificationAttempt> builder)
    {
        builder.ToTable("NotificationAttempts", "notification");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AttemptNumber).IsRequired();
        builder.Property(a => a.FailureReason).HasMaxLength(2048);

        builder.HasIndex(a => a.NotificationId)
            .HasDatabaseName("IX_NotificationAttempts_NotificationId");
    }
}
