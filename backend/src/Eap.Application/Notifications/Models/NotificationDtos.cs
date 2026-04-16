using Eap.Domain.Notifications;

namespace Eap.Application.Notifications.Models;

/// <summary>Summary row for the notification log table.</summary>
public sealed class NotificationSummaryDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string RecipientEmail { get; init; } = default!;
    public string? RecipientName { get; init; }
    public NotificationType NotificationType { get; init; }
    public NotificationStatus Status { get; init; }
    public string RelatedEntityType { get; init; } = default!;
    public Guid RelatedEntityId { get; init; }
    public string Subject { get; init; } = default!;
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? SentAtUtc { get; init; }
    public int AttemptCount { get; init; }
    public string? LastFailureReason { get; init; }
}

/// <summary>Filter parameters for notification listing.</summary>
public sealed record NotificationListFilter(
    int Page,
    int PageSize,
    NotificationType? Type = null,
    NotificationStatus? Status = null,
    string? Search = null);

/// <summary>Result of a notification send operation.</summary>
public sealed class NotificationResultDto
{
    public int TotalProcessed { get; init; }
    public int Sent { get; init; }
    public int Failed { get; init; }
    public int Skipped { get; init; }
}
