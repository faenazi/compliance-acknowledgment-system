using Eap.Application.Forms.Models;
using Eap.Application.UserPortal.Abstractions;
using Eap.Application.UserPortal.Models;
using Eap.Domain.Forms;
using Eap.Domain.Requirements;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.UserPortal.Persistence;

internal sealed class UserPortalRepository : IUserPortalRepository
{
    private readonly EapDbContext _db;

    public UserPortalRepository(EapDbContext db)
    {
        _db = db;
    }

    public async Task<MyDashboardDto> GetDashboardAsync(
        Guid userId, int pendingLimit, int recentLimit, CancellationToken ct)
    {
        var requirements = _db.UserActionRequirements
            .Where(r => r.UserId == userId && r.IsCurrent);

        var pendingCount = await requirements.CountAsync(r => r.Status == UserActionRequirementStatus.Pending, ct);
        var overdueCount = await requirements.CountAsync(r => r.Status == UserActionRequirementStatus.Overdue, ct);
        var completedCount = await requirements.CountAsync(r => r.Status == UserActionRequirementStatus.Completed, ct);

        var pendingActions = await BuildActionSummaryQuery(userId)
            .Where(dto => dto.Status == UserActionRequirementStatus.Pending
                       || dto.Status == UserActionRequirementStatus.Overdue)
            .OrderBy(dto => dto.DueDate.HasValue ? 0 : 1)
            .ThenBy(dto => dto.DueDate)
            .ThenByDescending(dto => dto.Status) // Overdue (2) before Pending (0)
            .Take(pendingLimit)
            .ToListAsync(ct);

        var recentlyCompleted = await BuildActionSummaryQuery(userId)
            .Where(dto => dto.Status == UserActionRequirementStatus.Completed)
            .OrderByDescending(dto => dto.CompletedAtUtc)
            .Take(recentLimit)
            .ToListAsync(ct);

        return new MyDashboardDto
        {
            PendingCount = pendingCount,
            OverdueCount = overdueCount,
            CompletedCount = completedCount,
            PendingActions = pendingActions,
            RecentlyCompleted = recentlyCompleted,
        };
    }

    public async Task<(IReadOnlyList<MyActionSummaryDto> Items, int TotalCount)> ListActionsAsync(
        Guid userId, MyActionsFilter filter, CancellationToken ct)
    {
        var query = BuildActionSummaryQuery(userId);

        if (filter.Status is { } status)
        {
            query = query.Where(dto => dto.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(dto =>
                EF.Functions.Like(dto.Title, $"%{search}%") ||
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

    public async Task<MyActionDetailDto?> GetActionDetailAsync(
        Guid userId, Guid requirementId, CancellationToken ct)
    {
        var row = await (
            from r in _db.UserActionRequirements.AsNoTracking()
            join av in _db.AcknowledgmentVersions.AsNoTracking() on r.AcknowledgmentVersionId equals av.Id
            join ad in _db.AcknowledgmentDefinitions.AsNoTracking() on av.AcknowledgmentDefinitionId equals ad.Id
            join pv in _db.PolicyVersions.AsNoTracking() on av.PolicyVersionId equals pv.Id
            join p in _db.Policies.AsNoTracking() on pv.PolicyId equals p.Id
            where r.Id == requirementId && r.UserId == userId
            select new
            {
                Req = r,
                AckVer = av,
                AckDef = ad,
                PolVer = pv,
                Policy = p,
                HasDoc = _db.PolicyDocuments.Any(d => d.PolicyVersionId == pv.Id),
            }
        ).FirstOrDefaultAsync(ct);

        if (row is null)
            return null;

        FormDefinitionDto? formDefDto = null;
        if (row.AckVer.ActionType == Domain.Acknowledgment.ActionType.FormBasedDisclosure)
        {
            var formDef = await _db.FormDefinitions
                .AsNoTracking()
                .Include(f => f.Fields.OrderBy(ff => ff.SortOrder))
                .FirstOrDefaultAsync(f => f.AcknowledgmentVersionId == row.AckVer.Id && f.IsActive, ct);

            if (formDef is not null)
            {
                formDefDto = new FormDefinitionDto
                {
                    Id = formDef.Id,
                    AcknowledgmentVersionId = formDef.AcknowledgmentVersionId,
                    SchemaVersion = formDef.SchemaVersion,
                    IsActive = formDef.IsActive,
                    Fields = formDef.Fields.Select(f => new FormFieldDto
                    {
                        Id = f.Id,
                        FieldKey = f.FieldKey,
                        Label = f.Label,
                        FieldType = f.FieldType,
                        IsRequired = f.IsRequired,
                        SortOrder = f.SortOrder,
                        SectionKey = f.SectionKey,
                        HelpText = f.HelpText,
                        Placeholder = f.Placeholder,
                        DisplayText = f.DisplayText,
                        Options = f.Options.Select(o => new FieldOptionDto { Value = o.Value, Label = o.Label }).ToList(),
                    }).ToList(),
                    CreatedAtUtc = formDef.CreatedAtUtc,
                    CreatedBy = formDef.CreatedBy,
                    UpdatedAtUtc = formDef.UpdatedAtUtc,
                    UpdatedBy = formDef.UpdatedBy,
                };
            }
        }

        // Check for existing submission
        var existingSubmission = await _db.UserSubmissions
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.UserActionRequirementId == requirementId)
            .OrderByDescending(s => s.SubmittedAtUtc)
            .Select(s => new { s.Id, s.SubmittedAtUtc })
            .FirstOrDefaultAsync(ct);

        return new MyActionDetailDto
        {
            RequirementId = row.Req.Id,
            Status = row.Req.Status,
            DueDate = row.Req.DueDate,
            AssignedAtUtc = row.Req.AssignedAtUtc,
            CompletedAtUtc = row.Req.CompletedAtUtc,
            CycleReference = row.Req.CycleReference,

            AcknowledgmentVersionId = row.AckVer.Id,
            AcknowledgmentDefinitionId = row.AckDef.Id,
            Title = row.AckDef.Title,
            Description = row.AckDef.Description,
            ActionType = row.AckVer.ActionType,
            RecurrenceModel = row.AckVer.RecurrenceModel,
            OwnerDepartment = row.AckDef.OwnerDepartment,
            Summary = row.AckVer.Summary,
            CommitmentText = row.AckVer.CommitmentText,
            StartDate = row.AckVer.StartDate,

            PolicyVersionId = row.PolVer.Id,
            PolicyId = row.Policy.Id,
            PolicyTitle = row.Policy.Title,
            PolicySummary = row.PolVer.Summary,
            PolicyVersionNumber = row.PolVer.VersionNumber,
            PolicyVersionLabel = row.PolVer.VersionLabel,
            HasPolicyDocument = row.HasDoc,

            FormDefinition = formDefDto,

            SubmissionId = existingSubmission?.Id,
            SubmittedAtUtc = existingSubmission?.SubmittedAtUtc,
        };
    }

    public async Task<(IReadOnlyList<MyHistoryItemDto> Items, int TotalCount)> ListHistoryAsync(
        Guid userId, int page, int pageSize, CancellationToken ct)
    {
        var query = from s in _db.UserSubmissions.AsNoTracking()
                    join av in _db.AcknowledgmentVersions.AsNoTracking() on s.AcknowledgmentVersionId equals av.Id
                    join ad in _db.AcknowledgmentDefinitions.AsNoTracking() on av.AcknowledgmentDefinitionId equals ad.Id
                    join pv in _db.PolicyVersions.AsNoTracking() on av.PolicyVersionId equals pv.Id
                    join p in _db.Policies.AsNoTracking() on pv.PolicyId equals p.Id
                    where s.UserId == userId
                    select new MyHistoryItemDto
                    {
                        SubmissionId = s.Id,
                        RequirementId = s.UserActionRequirementId ?? Guid.Empty,
                        Title = ad.Title,
                        PolicyTitle = p.Title,
                        ActionType = av.ActionType,
                        VersionNumber = av.VersionNumber,
                        SubmittedAtUtc = s.SubmittedAtUtc,
                        IsLateSubmission = s.IsLateSubmission,
                        CycleReference = _db.UserActionRequirements
                            .Where(r => r.Id == s.UserActionRequirementId)
                            .Select(r => r.CycleReference)
                            .FirstOrDefault() ?? "",
                    };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(dto => dto.SubmittedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<MySubmissionDetailDto?> GetSubmissionDetailAsync(
        Guid userId, Guid submissionId, CancellationToken ct)
    {
        var submission = await _db.UserSubmissions
            .AsNoTracking()
            .Include(s => s.FieldValues)
            .FirstOrDefaultAsync(s => s.Id == submissionId && s.UserId == userId, ct);

        if (submission is null)
            return null;

        var context = await (
            from av in _db.AcknowledgmentVersions.AsNoTracking()
            join ad in _db.AcknowledgmentDefinitions.AsNoTracking() on av.AcknowledgmentDefinitionId equals ad.Id
            join pv in _db.PolicyVersions.AsNoTracking() on av.PolicyVersionId equals pv.Id
            join p in _db.Policies.AsNoTracking() on pv.PolicyId equals p.Id
            where av.Id == submission.AcknowledgmentVersionId
            select new
            {
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

        var requirementId = submission.UserActionRequirementId ?? Guid.Empty;
        var cycleRef = "";
        if (submission.UserActionRequirementId.HasValue)
        {
            cycleRef = await _db.UserActionRequirements
                .Where(r => r.Id == submission.UserActionRequirementId)
                .Select(r => r.CycleReference)
                .FirstOrDefaultAsync(ct) ?? "";
        }

        return new MySubmissionDetailDto
        {
            SubmissionId = submission.Id,
            RequirementId = requirementId,
            SubmittedAtUtc = submission.SubmittedAtUtc,
            IsLateSubmission = submission.IsLateSubmission,

            Title = context.Title,
            Description = context.Description,
            ActionType = context.ActionType,
            OwnerDepartment = context.OwnerDepartment,
            VersionNumber = context.VersionNumber,
            CommitmentText = context.CommitmentText,

            PolicyTitle = context.PolicyTitle,
            PolicyVersionNumber = context.PolicyVersionNumber,
            PolicyVersionLabel = context.PolicyVersionLabel,

            SubmissionJson = submission.SubmissionJson,
            FormDefinitionSnapshotJson = submission.FormDefinitionSnapshotJson,
            FieldValues = submission.FieldValues.Select(fv => new SubmissionFieldValueDto
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

    public async Task<UserActionRequirement?> FindOpenRequirementAsync(
        Guid userId, Guid requirementId, CancellationToken ct)
    {
        return await _db.UserActionRequirements
            .FirstOrDefaultAsync(r =>
                r.Id == requirementId &&
                r.UserId == userId &&
                (r.Status == UserActionRequirementStatus.Pending || r.Status == UserActionRequirementStatus.Overdue),
                ct);
    }

    public async Task<bool> HasSubmissionForRequirementAsync(
        Guid userId, Guid requirementId, CancellationToken ct)
    {
        return await _db.UserSubmissions
            .AnyAsync(s => s.UserId == userId && s.UserActionRequirementId == requirementId, ct);
    }

    public async Task<UserSubmission?> FindSubmissionByRequirementAsync(
        Guid userId, Guid requirementId, CancellationToken ct)
    {
        return await _db.UserSubmissions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.UserActionRequirementId == requirementId, ct);
    }

    public async Task AddSubmissionAsync(UserSubmission submission, CancellationToken ct)
    {
        await _db.UserSubmissions.AddAsync(submission, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    /// <summary>Builds the base projection query that joins requirements → acknowledgments → policies.</summary>
    private IQueryable<MyActionSummaryDto> BuildActionSummaryQuery(Guid userId)
    {
        return from r in _db.UserActionRequirements.AsNoTracking()
               join av in _db.AcknowledgmentVersions.AsNoTracking() on r.AcknowledgmentVersionId equals av.Id
               join ad in _db.AcknowledgmentDefinitions.AsNoTracking() on av.AcknowledgmentDefinitionId equals ad.Id
               join pv in _db.PolicyVersions.AsNoTracking() on av.PolicyVersionId equals pv.Id
               join p in _db.Policies.AsNoTracking() on pv.PolicyId equals p.Id
               where r.UserId == userId && r.IsCurrent
               select new MyActionSummaryDto
               {
                   RequirementId = r.Id,
                   Title = ad.Title,
                   PolicyTitle = p.Title,
                   ActionType = av.ActionType,
                   RecurrenceModel = av.RecurrenceModel,
                   OwnerDepartment = ad.OwnerDepartment,
                   DueDate = r.DueDate,
                   Status = r.Status,
                   AssignedAtUtc = r.AssignedAtUtc,
                   CompletedAtUtc = r.CompletedAtUtc,
                   CycleReference = r.CycleReference,
               };
    }
}
