using Eap.Application.Compliance.Abstractions;
using Eap.Application.Compliance.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Requirements;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Compliance.Persistence;

/// <summary>
/// Read-optimized compliance repository (Sprint 8). Provides aggregate compliance
/// metrics from cross-entity LINQ joins across Users, UserActionRequirements,
/// AcknowledgmentVersions, AcknowledgmentDefinitions, and Policies.
/// </summary>
internal sealed class ComplianceRepository : IComplianceRepository
{
    private readonly EapDbContext _db;

    public ComplianceRepository(EapDbContext db)
    {
        _db = db;
    }

    public async Task<ComplianceDashboardDto> GetDashboardAsync(
        ComplianceDashboardFilter filter, CancellationToken ct)
    {
        var requirementsQuery = _db.UserActionRequirements
            .Where(r => r.IsCurrent);

        if (!string.IsNullOrWhiteSpace(filter.Department))
        {
            requirementsQuery = requirementsQuery.Where(r =>
                _db.Users.Any(u => u.Id == r.UserId
                    && u.Department != null
                    && u.Department.Contains(filter.Department)));
        }

        if (filter.AcknowledgmentDefinitionId.HasValue)
        {
            requirementsQuery = requirementsQuery.Where(r =>
                _db.AcknowledgmentVersions.Any(v =>
                    v.Id == r.AcknowledgmentVersionId
                    && _db.AcknowledgmentDefinitions.Any(d =>
                        d.Id == v.AcknowledgmentDefinitionId
                        && d.Id == filter.AcknowledgmentDefinitionId.Value)));
        }

        if (filter.PolicyId.HasValue)
        {
            requirementsQuery = requirementsQuery.Where(r =>
                _db.AcknowledgmentVersions.Any(v =>
                    v.Id == r.AcknowledgmentVersionId
                    && _db.PolicyVersions.Any(pv =>
                        pv.Id == v.PolicyVersionId
                        && pv.PolicyId == filter.PolicyId.Value)));
        }

        var total = await requirementsQuery.CountAsync(ct);
        var completed = await requirementsQuery
            .CountAsync(r => r.Status == UserActionRequirementStatus.Completed, ct);
        var pending = await requirementsQuery
            .CountAsync(r => r.Status == UserActionRequirementStatus.Pending, ct);
        var overdue = await requirementsQuery
            .CountAsync(r => r.Status == UserActionRequirementStatus.Overdue, ct);
        var completionRate = total > 0
            ? Math.Round((decimal)completed / total * 100, 1)
            : 0m;

        var byDepartment = await GetDepartmentComplianceCoreAsync(
            requirementsQuery, ct);

        var byAction = await GetActionComplianceCoreAsync(
            requirementsQuery, ct);

        var topNonCompliant = await GetTopNonCompliantUsersAsync(
            requirementsQuery, filter.TopNonCompliantLimit, ct);

        return new ComplianceDashboardDto
        {
            TotalRequirements = total,
            CompletedRequirements = completed,
            PendingRequirements = pending,
            OverdueRequirements = overdue,
            CompletionRate = completionRate,
            ComplianceByDepartment = byDepartment,
            ComplianceByAction = byAction,
            TopNonCompliantUsers = topNonCompliant,
        };
    }

    public async Task<(IReadOnlyList<NonCompliantUserDetailDto> Items, int TotalCount)>
        ListNonCompliantUsersAsync(ComplianceReportFilter filter, CancellationToken ct)
    {
        var query = from r in _db.UserActionRequirements
                    where r.IsCurrent
                        && (r.Status == UserActionRequirementStatus.Pending
                            || r.Status == UserActionRequirementStatus.Overdue)
                    join u in _db.Users on r.UserId equals u.Id
                    join av in _db.AcknowledgmentVersions on r.AcknowledgmentVersionId equals av.Id
                    join ad in _db.AcknowledgmentDefinitions on av.AcknowledgmentDefinitionId equals ad.Id
                    select new { r, u, av, ad };

        if (!string.IsNullOrWhiteSpace(filter.Department))
        {
            query = query.Where(x =>
                x.u.Department != null && x.u.Department.Contains(filter.Department));
        }

        if (filter.AcknowledgmentDefinitionId.HasValue)
        {
            query = query.Where(x => x.ad.Id == filter.AcknowledgmentDefinitionId.Value);
        }

        if (filter.PolicyId.HasValue)
        {
            query = query.Where(x =>
                _db.PolicyVersions.Any(pv =>
                    pv.Id == x.av.PolicyVersionId
                    && pv.PolicyId == filter.PolicyId.Value));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.r.Status == filter.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.Trim();
            query = query.Where(x =>
                x.u.DisplayName.Contains(searchTerm)
                || x.u.Username.Contains(searchTerm)
                || x.ad.Title.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.r.Status == UserActionRequirementStatus.Overdue ? 0 : 1)
            .ThenBy(x => x.r.DueDate)
            .ThenBy(x => x.u.DisplayName)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new NonCompliantUserDetailDto
            {
                UserId = x.u.Id,
                DisplayName = x.u.DisplayName,
                Department = x.u.Department ?? string.Empty,
                Email = x.u.Email,
                RequirementId = x.r.Id,
                AcknowledgmentDefinitionId = x.ad.Id,
                AcknowledgmentVersionId = x.av.Id,
                ActionTitle = x.ad.Title,
                ActionType = x.ad.DefaultActionType,
                Status = x.r.Status,
                DueDate = x.r.DueDate,
                AssignedAtUtc = x.r.AssignedAtUtc,
                CycleReference = x.r.CycleReference,
            })
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<DepartmentComplianceDto>> GetDepartmentComplianceAsync(
        string? department, Guid? acknowledgmentDefinitionId, Guid? policyId, CancellationToken ct)
    {
        var query = _db.UserActionRequirements.Where(r => r.IsCurrent);

        if (acknowledgmentDefinitionId.HasValue)
        {
            query = query.Where(r =>
                _db.AcknowledgmentVersions.Any(v =>
                    v.Id == r.AcknowledgmentVersionId
                    && _db.AcknowledgmentDefinitions.Any(d =>
                        d.Id == v.AcknowledgmentDefinitionId
                        && d.Id == acknowledgmentDefinitionId.Value)));
        }

        if (policyId.HasValue)
        {
            query = query.Where(r =>
                _db.AcknowledgmentVersions.Any(v =>
                    v.Id == r.AcknowledgmentVersionId
                    && _db.PolicyVersions.Any(pv =>
                        pv.Id == v.PolicyVersionId
                        && pv.PolicyId == policyId.Value)));
        }

        return await GetDepartmentComplianceCoreAsync(query, ct);
    }

    public async Task<IReadOnlyList<ActionComplianceDto>> GetActionComplianceAsync(
        string? department, Guid? policyId, CancellationToken ct)
    {
        var query = _db.UserActionRequirements.Where(r => r.IsCurrent);

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(r =>
                _db.Users.Any(u => u.Id == r.UserId
                    && u.Department != null
                    && u.Department.Contains(department)));
        }

        if (policyId.HasValue)
        {
            query = query.Where(r =>
                _db.AcknowledgmentVersions.Any(v =>
                    v.Id == r.AcknowledgmentVersionId
                    && _db.PolicyVersions.Any(pv =>
                        pv.Id == v.PolicyVersionId
                        && pv.PolicyId == policyId.Value)));
        }

        return await GetActionComplianceCoreAsync(query, ct);
    }

    // ────────────────────── private helpers ──────────────────────

    private async Task<IReadOnlyList<DepartmentComplianceDto>> GetDepartmentComplianceCoreAsync(
        IQueryable<UserActionRequirement> baseQuery, CancellationToken ct)
    {
        var joined = from r in baseQuery
                     join u in _db.Users on r.UserId equals u.Id
                     select new { r.Status, Department = u.Department ?? "غير محدد" };

        return await joined
            .GroupBy(x => x.Department)
            .Select(g => new DepartmentComplianceDto
            {
                Department = g.Key,
                TotalAssigned = g.Count(),
                Completed = g.Count(x => x.Status == UserActionRequirementStatus.Completed),
                Pending = g.Count(x => x.Status == UserActionRequirementStatus.Pending),
                Overdue = g.Count(x => x.Status == UserActionRequirementStatus.Overdue),
                CompletionRate = g.Count() > 0
                    ? Math.Round(
                        (decimal)g.Count(x => x.Status == UserActionRequirementStatus.Completed)
                        / g.Count() * 100, 1)
                    : 0m,
            })
            .OrderByDescending(d => d.TotalAssigned)
            .ToListAsync(ct);
    }

    private async Task<IReadOnlyList<ActionComplianceDto>> GetActionComplianceCoreAsync(
        IQueryable<UserActionRequirement> baseQuery, CancellationToken ct)
    {
        var joined = from r in baseQuery
                     join av in _db.AcknowledgmentVersions on r.AcknowledgmentVersionId equals av.Id
                     join ad in _db.AcknowledgmentDefinitions on av.AcknowledgmentDefinitionId equals ad.Id
                     select new
                     {
                         r.Status,
                         ad.Id,
                         ad.Title,
                         ActionType = ad.DefaultActionType,
                         ad.OwnerDepartment,
                     };

        return await joined
            .GroupBy(x => new { x.Id, x.Title, x.ActionType, x.OwnerDepartment })
            .Select(g => new ActionComplianceDto
            {
                AcknowledgmentDefinitionId = g.Key.Id,
                ActionTitle = g.Key.Title,
                ActionType = g.Key.ActionType,
                OwnerDepartment = g.Key.OwnerDepartment,
                TotalAssigned = g.Count(),
                Completed = g.Count(x => x.Status == UserActionRequirementStatus.Completed),
                Pending = g.Count(x => x.Status == UserActionRequirementStatus.Pending),
                Overdue = g.Count(x => x.Status == UserActionRequirementStatus.Overdue),
                CompletionRate = g.Count() > 0
                    ? Math.Round(
                        (decimal)g.Count(x => x.Status == UserActionRequirementStatus.Completed)
                        / g.Count() * 100, 1)
                    : 0m,
            })
            .OrderByDescending(a => a.TotalAssigned)
            .ToListAsync(ct);
    }

    private async Task<IReadOnlyList<NonCompliantUserSummaryDto>> GetTopNonCompliantUsersAsync(
        IQueryable<UserActionRequirement> baseQuery, int limit, CancellationToken ct)
    {
        var nonCompliant = from r in baseQuery
                           where r.Status == UserActionRequirementStatus.Pending
                               || r.Status == UserActionRequirementStatus.Overdue
                           join u in _db.Users on r.UserId equals u.Id
                           select new { u, r.Status };

        return await nonCompliant
            .GroupBy(x => new { x.u.Id, x.u.DisplayName, Department = x.u.Department ?? "غير محدد", x.u.Email })
            .Select(g => new NonCompliantUserSummaryDto
            {
                UserId = g.Key.Id,
                DisplayName = g.Key.DisplayName,
                Department = g.Key.Department,
                Email = g.Key.Email,
                PendingCount = g.Count(x => x.Status == UserActionRequirementStatus.Pending),
                OverdueCount = g.Count(x => x.Status == UserActionRequirementStatus.Overdue),
                TotalNonCompliant = g.Count(),
            })
            .OrderByDescending(x => x.OverdueCount)
            .ThenByDescending(x => x.PendingCount)
            .Take(limit)
            .ToListAsync(ct);
    }
}
