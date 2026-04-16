using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Notifications.Persistence;

internal sealed class NotificationRepository : INotificationRepository
{
    private readonly EapDbContext _db;

    public NotificationRepository(EapDbContext db)
    {
        _db = db;
    }

    public async Task<Notification> AddAsync(Notification notification, CancellationToken ct)
    {
        await _db.Notifications.AddAsync(notification, ct);
        return notification;
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public Task<bool> ExistsAsync(
        Guid userId, NotificationType type, Guid relatedEntityId, CancellationToken ct)
    {
        return _db.Notifications.AnyAsync(n =>
            n.UserId == userId
            && n.NotificationType == type
            && n.RelatedEntityId == relatedEntityId
            && (n.Status == NotificationStatus.Sent || n.Status == NotificationStatus.Queued), ct);
    }

    public async Task<(IReadOnlyList<NotificationSummaryDto> Items, int TotalCount)> ListAsync(
        NotificationListFilter filter, CancellationToken ct)
    {
        var query = _db.Notifications.AsQueryable();

        if (filter.Type.HasValue)
            query = query.Where(n => n.NotificationType == filter.Type.Value);

        if (filter.Status.HasValue)
            query = query.Where(n => n.Status == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(n =>
                n.RecipientEmail.Contains(term) || n.Subject.Contains(term));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(n => n.CreatedAtUtc)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(n => new NotificationSummaryDto
            {
                Id = n.Id,
                UserId = n.UserId,
                RecipientEmail = n.RecipientEmail,
                RecipientName = _db.Users
                    .Where(u => u.Id == n.UserId)
                    .Select(u => u.FullName)
                    .FirstOrDefault(),
                NotificationType = n.NotificationType,
                Status = n.Status,
                RelatedEntityType = n.RelatedEntityType,
                RelatedEntityId = n.RelatedEntityId,
                Subject = n.Subject,
                CreatedAtUtc = n.CreatedAtUtc,
                SentAtUtc = n.SentAtUtc,
                AttemptCount = n.Attempts.Count(),
                LastFailureReason = n.Attempts
                    .OrderByDescending(a => a.AttemptNumber)
                    .Where(a => !a.Success)
                    .Select(a => a.FailureReason)
                    .FirstOrDefault(),
            })
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
