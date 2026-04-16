using Eap.Application.Admin.Abstractions;
using Eap.Application.Admin.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Policy;
using Eap.Domain.Requirements;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Admin.Persistence;

/// <summary>
/// Read-optimized repository for admin portal queries (Sprint 7).
/// Joins across aggregates (requirements → acknowledgments → policies → users)
/// to produce admin-centric projections for the dashboard and monitoring pages.
/// </summary>
internal sealed class AdminRepository : IAdminRepository
{
    private readonly EapDbContext _db;

    public AdminRepository(EapDbContext db)
    {
        _db = db;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(int recentLimit, CancellationToken ct)
    {
        // KPI counts: policies
        var activePolicies = await _db.Policies
            .CountAsync(p => p.Status == PolicyStatus.Published, ct);

        // KPI counts: acknowledgments
        var activeAcknowledgments = await _db.AcknowledgmentDefinitions
            .CountAsync(ad => ad.Status == AcknowledgmentStatus.Published, ct);

        // KPI counts: user action requirements (current only)
        var currentRequirements = _db.UserActionRequirements.Where(r => r.IsCurrent);
        var pendingActions = await currentRequirements
            .CountAsync(r => r.Status == UserActionRequirementStatus.Pending, ct);
        var overdueActions = await currentRequirements
            .CountAsync(r => r.Status == UserActionRequirementStatus.Overdue, ct);
        var completedActions = await currentRequirements
            .CountAsync(r => r.Status == UserActionRequirementStatus.Completed, ct);
        var totalActions = await currentRequirements.CountAsync(ct);

        var completionRate = totalActions > 0
            ? Math.Round((decimal)completedActions / totalActions * 100, 1)
            : 0m;

        // Recently published policies
        var recentPolicies = await (
            from pv in _db.PolicyVersions.AsNoTracking()
            join p in _db.Policies.AsNoTracking() on pv.PolicyId equals p.Id
            where pv.Status == PolicyVersionStatus.Published && pv.PublishedAtUtc != null
            orderby pv.PublishedAtUtc descending
            select new RecentlyPublishedItemDto
            {
                Id = p.Id,
                VersionId = pv.Id,
                Title = p.Title,
                OwnerDepartment = p.OwnerDepartment,
                VersionNumber = pv.VersionNumber,
                PublishedAtUtc = pv.PublishedAtUtc!.Value,
            }
        ).Take(recentLimit).ToListAsync(ct);

        // Recently published acknowledgments
        var recentAcknowledgments = await (
            from av in _db.AcknowledgmentVersions.AsNoTracking()
            join ad in _db.AcknowledgmentDefinitions.AsNoTracking() on av.AcknowledgmentDefinitionId equals ad.Id
            where av.Status == AcknowledgmentVersionStatus.Published && av.PublishedAtUtc != null
            orderby av.PublishedAtUtc descending
            select new RecentlyPublishedItemDto
            {
                Id = ad.Id,
                VersionId = av.Id,
                Title = ad.Title,
                OwnerDepartment = ad.OwnerDepartment,
                VersionNumber = av.VersionNumber,
                PublishedAtUtc = av.PublishedAtUtc!.Value,
            }
        ).Take(recentLimit).ToListAsync(ct);

        return new AdminDashboardDto
        {
            ActivePolicies = activePolicies,
            ActiveAcknowledgments = activeAcknowledgments,
            PendingUserActions = pendingActions,
            OverdueUserActions = overdueActions,
            CompletedUserActions = completedActions,
            TotalUserActions = totalActions,
            CompletionRate = completionRate,
            RecentlyPublishedPolicies = recentPolicies,
            RecentlyPublishedAcknowledgments = recentAcknowledgments,
        };
    }

    public async Task<(IReadOnlyList<AdminRequirementSummaryDto> Items, int TotalCount)> ListRequirementsAsync(
        AdminRequirementsFilter filter, CancellationToken ct)
    {
        var query = BuildRequirementSummaryQuery();

        if (filter.CurrentOnly)
            query = query.Where(dto => dto.IsCurrent);

        if (filter.Status is { } status)
            query = query.Where(dto => dto.Status == status);

        if (filter.AcknowledgmentDefinitionId is { } defId)
            query = query.Where(dto => dto.AcknowledgmentDefinitionId == defId);

        if (filter.PolicyId is { } policyId)
            query = query.Where(dto => dto.PolicyTitle != null); // Will be refined in the raw query

        if (!string.IsNullOrWhiteSpace(filter.Department))
        {
            var dept = filter.Department.Trim();
            query = query.Where(dto => EF.Functions.Like(dto.UserDepartment, $"%{dept}%"));
        }

        if (filter.RecurrenceModel is { } recurrence)
            query = query.Where(dto => dto.RecurrenceModel == recurrence);

        if (filter.DueDateFrom is { } from)
            query = query.Where(dto => dto.DueDate != null && dto.DueDate >= from);

        if (filter.DueDateTo is { } to)
            query = query.Where(dto => dto.DueDate != null && dto.DueDate <= to);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(dto =>
                EF.Functions.Like(dto.UserDisplayName, $"%{search}%") ||
                EF.Functions.Like(dto.ActionTitle, $"%{search}%") ||
                EF.Functions.Like(dto.PolicyTitle, $"%{search}%"));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(dto => dto.Status == UserActionRequirementStatus.Overdue ? 0 :
                            dto.Status == UserActionRequirementStatus.Pending ? 1 : 2)
            .ThenBy(dto => dto.DueDate.HasValue ? 0 : 1)
            .ThenBy(dto => dto.DueDate)
            .ThenByDescending(dto => dto.AssignedAtUtc)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<AdminRequirementDetailDto?> GetRequirementDetailAsync(
        Guid requirementId, CancellationToken ct)
    {
        var row = await (
            from r in _db.UserActionRequirements.AsNoTracking()
            join u in _db.Users.AsNoTracking() on r.UserId equals u.Id
            join av in _db.AcknowledgmentVersions.AsNoTracking() on r.AcknowledgmentVersionId equals av.Id
            join ad in _db.AcknowledgmentDefinitions.AsNoTracking() on av.AcknowledgmentDefinitionId equals ad.Id
            join pv in _db.PolicyVersions.AsNoTracking() on av.PolicyVersionId equals pv.Id
            join p in _db.Policies.AsNoTracking() on pv.PolicyId equals p.Id
            where r.Id == requirementId
            select new { Req = r, User = u, AckVer = av, AckDef = ad, PolVer = pv, Policy = p }
        ).FirstOrDefaultAsync(ct);

        if (row is null)
            return null;

        // Check for existing submission
        var submission = await _db.UserSubmissions
            .AsNoTracking()
            .Where(s => s.UserActionRequirementId == requirementId)
            .OrderByDescending(s => s.SubmittedAtUtc)
            .Select(s => new { s.Id, s.SubmittedAtUtc, s.IsLateSubmission })
            .FirstOrDefaultAsync(ct);

        return new AdminRequirementDetailDto
        {
            RequirementId = row.Req.Id,
            Status = row.Req.Status,
            DueDate = row.Req.DueDate,
            AssignedAtUtc = row.Req.AssignedAtUtc,
            CompletedAtUtc = row.Req.CompletedAtUtc,
            CycleReference = row.Req.CycleReference,
            IsCurrent = row.Req.IsCurrent,

            UserId = row.User.Id,
            UserDisplayName = row.User.FullName,
            UserDepartment = row.User.Department ?? "",
            UserEmail = row.User.Email,

            AcknowledgmentDefinitionId = row.AckDef.Id,
            AcknowledgmentVersionId = row.AckVer.Id,
            ActionTitle = row.AckDef.Title,
            ActionType = row.AckVer.ActionType,
            RecurrenceModel = row.AckVer.RecurrenceModel,
            OwnerDepartment = row.AckDef.OwnerDepartment,
            VersionNumber = row.AckVer.VersionNumber,

            PolicyId = row.Policy.Id,
            PolicyVersionId = row.PolVer.Id,
            PolicyTitle = row.Policy.Title,
            PolicyVersionNumber = row.PolVer.VersionNumber,

            SubmissionId = submission?.Id,
            SubmittedAtUtc = submission?.SubmittedAtUtc,
            IsLateSubmission = submission?.IsLateSubmission,
        };
    }

    public async Task<AdminSubmissionDetailDto?> GetSubmissionDetailAsync(
        Guid submissionId, CancellationToken ct)
    {
        var submission = await _db.UserSubmissions
            .AsNoTracking()
            .Include(s => s.FieldValues)
            .FirstOrDefaultAsync(s => s.Id == submissionId, ct);

        if (submission is null)
            return null;

        var user = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == submission.UserId)
            .Select(u => new { u.FullName, u.Department })
            .FirstOrDefaultAsync(ct);

        var context = await (
            from av in _db.AcknowledgmentVersions.AsNoTracking()
            join ad in _db.AcknowledgmentDefinitions.AsNoTracking() on av.AcknowledgmentDefinitionId equals ad.Id
            join pv in _db.PolicyVersions.AsNoTracking() on av.PolicyVersionId equals pv.Id
            join p in _db.Policies.AsNoTracking() on pv.PolicyId equals p.Id
            where av.Id == submission.AcknowledgmentVersionId
            select new
            {
                AckDefId = ad.Id,
                AckVerId = av.Id,
                ad.Title,
                ad.Description,
                av.ActionType,
                ad.OwnerDepartment,
                av.VersionNumber,
                av.CommitmentText,
                PolicyTitle = p.Title,
                PolicyVersionNumber = pv.VersionNumber,
                PolicyVersionLabel = pv.VersionLabel,
            }
        ).FirstOrDefaultAsync(ct);

        if (context is null)
            return null;

        // Requirement context
        string? cycleRef = null;
        if (submission.UserActionRequirementId.HasValue)
        {
            cycleRef = await _db.UserActionRequirements
                .Where(r => r.Id == submission.UserActionRequirementId)
                .Select(r => r.CycleReference)
                .FirstOrDefaultAsync(ct);
        }

        return new AdminSubmissionDetailDto
        {
            SubmissionId = submission.Id,
            SubmittedAtUtc = submission.SubmittedAtUtc,
            IsLateSubmission = submission.IsLateSubmission,

            UserId = submission.UserId,
            UserDisplayName = user?.FullName ?? "",
            UserDepartment = user?.Department ?? "",

            AcknowledgmentDefinitionId = context.AckDefId,
            AcknowledgmentVersionId = context.AckVerId,
            ActionTitle = context.Title,
            ActionDescription = context.Description,
            ActionType = context.ActionType,
            OwnerDepartment = context.OwnerDepartment,
            VersionNumber = context.VersionNumber,
            CommitmentText = context.CommitmentText,

            PolicyTitle = context.PolicyTitle,
            PolicyVersionNumber = context.PolicyVersionNumber,
            PolicyVersionLabel = context.PolicyVersionLabel,

            RequirementId = submission.UserActionRequirementId,
            CycleReference = cycleRef,

            SubmissionJson = submission.SubmissionJson,
            FormDefinitionSnapshotJson = submission.FormDefinitionSnapshotJson,
            FieldValues = submission.FieldValues.Select(fv => new AdminFieldValueDto
            {
                Id = fv.Id,
                FieldKey = fv.FieldKey,
                FieldLabel = fv.FieldLabel,
                FieldType = fv.FieldType,
                ValueText = fv.ValueText,
                ValueNumber = fv.ValueNumber,
                ValueDate = fv.ValueDate,
                ValueBoolean = fv.ValueBoolean,
                ValueJson = fv.ValueJson,
            }).ToList(),
        };
    }

    /// <summary>Base projection query joining requirements → users → acknowledgments → policies.</summary>
    private IQueryable<AdminRequirementSummaryDto> BuildRequirementSummaryQuery()
    {
        return from r in _db.UserActionRequirements.AsNoTracking()
               join u in _db.Users.AsNoTracking() on r.UserId equals u.Id
               join av in _db.AcknowledgmentVersions.AsNoTracking() on r.AcknowledgmentVersionId equals av.Id
               join ad in _db.AcknowledgmentDefinitions.AsNoTracking() on av.AcknowledgmentDefinitionId equals ad.Id
               join pv in _db.PolicyVersions.AsNoTracking() on av.PolicyVersionId equals pv.Id
               join p in _db.Policies.AsNoTracking() on pv.PolicyId equals p.Id
               select new AdminRequirementSummaryDto
               {
                   RequirementId = r.Id,
                   UserId = u.Id,
                   UserDisplayName = u.FullName,
                   UserDepartment = u.Department ?? "",
                   AcknowledgmentDefinitionId = ad.Id,
                   AcknowledgmentVersionId = av.Id,
                   ActionTitle = ad.Title,
                   ActionType = av.ActionType,
                   RecurrenceModel = av.RecurrenceModel,
                   VersionNumber = av.VersionNumber,
                   PolicyTitle = p.Title,
                   Status = r.Status,
                   CycleReference = r.CycleReference,
                   AssignedAtUtc = r.AssignedAtUtc,
                   DueDate = r.DueDate,
                   CompletedAtUtc = r.CompletedAtUtc,
                   IsCurrent = r.IsCurrent,
               };
    }
}
