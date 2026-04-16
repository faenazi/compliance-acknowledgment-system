using Eap.Application.Notifications.Models;

namespace Eap.Application.Notifications.Abstractions;

/// <summary>
/// Provides pre-built queries for notification candidate lookup.
/// Implemented by the Infrastructure layer to avoid Application depending on EF Core.
/// </summary>
public interface INotificationCandidateQuery
{
    Task<IReadOnlyList<NotificationCandidate>> GetOverdueCandidatesAsync(CancellationToken ct);

    Task<IReadOnlyList<NotificationCandidate>> GetPendingAssignmentCandidatesAsync(
        Guid? acknowledgmentVersionId, CancellationToken ct);

    Task<IReadOnlyList<NotificationCandidate>> GetReminderCandidatesAsync(
        DateOnly cutoffDate, CancellationToken ct);
}
