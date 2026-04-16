using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using Eap.Domain.Requirements;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Notifications.Persistence;

internal sealed class NotificationCandidateQuery : INotificationCandidateQuery
{
    private readonly EapDbContext _db;

    public NotificationCandidateQuery(EapDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<NotificationCandidate>> GetOverdueCandidatesAsync(CancellationToken ct)
    {
        return await (
            from r in _db.UserActionRequirements
            where r.IsCurrent && r.Status == UserActionRequirementStatus.Overdue
            join u in _db.Users on r.UserId equals u.Id
            join av in _db.AcknowledgmentVersions on r.AcknowledgmentVersionId equals av.Id
            join ad in _db.AcknowledgmentDefinitions on av.AcknowledgmentDefinitionId equals ad.Id
            select new NotificationCandidate(r.Id, u.Id, u.Email, r.DueDate, ad.Title)
        ).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<NotificationCandidate>> GetPendingAssignmentCandidatesAsync(
        Guid? acknowledgmentVersionId, CancellationToken ct)
    {
        var query = from r in _db.UserActionRequirements
                    where r.IsCurrent && r.Status == UserActionRequirementStatus.Pending
                    join u in _db.Users on r.UserId equals u.Id
                    join av in _db.AcknowledgmentVersions on r.AcknowledgmentVersionId equals av.Id
                    join ad in _db.AcknowledgmentDefinitions on av.AcknowledgmentDefinitionId equals ad.Id
                    select new { r, u, av, ad };

        if (acknowledgmentVersionId.HasValue)
        {
            query = query.Where(x => x.av.Id == acknowledgmentVersionId.Value);
        }

        return await query
            .Select(x => new NotificationCandidate(x.r.Id, x.u.Id, x.u.Email, x.r.DueDate, x.ad.Title))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<NotificationCandidate>> GetReminderCandidatesAsync(
        DateOnly cutoffDate, CancellationToken ct)
    {
        return await (
            from r in _db.UserActionRequirements
            where r.IsCurrent
                && r.Status == UserActionRequirementStatus.Pending
                && r.DueDate.HasValue
                && r.DueDate.Value <= cutoffDate
            join u in _db.Users on r.UserId equals u.Id
            join av in _db.AcknowledgmentVersions on r.AcknowledgmentVersionId equals av.Id
            join ad in _db.AcknowledgmentDefinitions on av.AcknowledgmentDefinitionId equals ad.Id
            select new NotificationCandidate(r.Id, u.Id, u.Email, r.DueDate, ad.Title)
        ).ToListAsync(ct);
    }
}
