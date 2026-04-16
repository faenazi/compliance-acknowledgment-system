using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;

namespace Eap.Application.Notifications.Abstractions;

/// <summary>
/// Repository for notification persistence and querying (Sprint 8).
/// </summary>
public interface INotificationRepository
{
    Task<Notification> AddAsync(Notification notification, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);

    /// <summary>Check if a notification of the given type already exists for the user/entity pair.</summary>
    Task<bool> ExistsAsync(
        Guid userId, NotificationType type, Guid relatedEntityId, CancellationToken ct);

    /// <summary>List notifications with filters (admin view).</summary>
    Task<(IReadOnlyList<NotificationSummaryDto> Items, int TotalCount)> ListAsync(
        NotificationListFilter filter, CancellationToken ct);
}
