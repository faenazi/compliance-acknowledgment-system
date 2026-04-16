using Eap.Application.Admin.Models;

namespace Eap.Application.Admin.Abstractions;

/// <summary>
/// Read-optimized repository for admin portal queries (Sprint 7).
/// Provides cross-entity projections for dashboard, monitoring, and
/// historical detail views. Separate from end-user repositories so
/// admin and user-scoped read paths stay independent.
/// </summary>
public interface IAdminRepository
{
    /// <summary>Returns the admin dashboard KPI summary.</summary>
    Task<AdminDashboardDto> GetDashboardAsync(int recentLimit, CancellationToken ct);

    /// <summary>Returns a paginated, filtered list of all user action requirements.</summary>
    Task<(IReadOnlyList<AdminRequirementSummaryDto> Items, int TotalCount)> ListRequirementsAsync(
        AdminRequirementsFilter filter, CancellationToken ct);

    /// <summary>Returns full detail for a single user action requirement (admin view).</summary>
    Task<AdminRequirementDetailDto?> GetRequirementDetailAsync(Guid requirementId, CancellationToken ct);

    /// <summary>Returns full detail for a submission (admin view, not scoped to current user).</summary>
    Task<AdminSubmissionDetailDto?> GetSubmissionDetailAsync(Guid submissionId, CancellationToken ct);
}
