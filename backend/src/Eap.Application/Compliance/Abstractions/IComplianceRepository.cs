using Eap.Application.Compliance.Models;

namespace Eap.Application.Compliance.Abstractions;

/// <summary>
/// Read-optimized repository for compliance reporting queries (Sprint 8).
/// Provides aggregate compliance metrics by department, action, and user.
/// </summary>
public interface IComplianceRepository
{
    /// <summary>Returns the compliance dashboard summary with optional filters.</summary>
    Task<ComplianceDashboardDto> GetDashboardAsync(ComplianceDashboardFilter filter, CancellationToken ct);

    /// <summary>Returns a paginated list of non-compliant user details.</summary>
    Task<(IReadOnlyList<NonCompliantUserDetailDto> Items, int TotalCount)> ListNonCompliantUsersAsync(
        ComplianceReportFilter filter, CancellationToken ct);

    /// <summary>Returns department-level compliance data with optional filters.</summary>
    Task<IReadOnlyList<DepartmentComplianceDto>> GetDepartmentComplianceAsync(
        string? department, Guid? acknowledgmentDefinitionId, Guid? policyId, CancellationToken ct);

    /// <summary>Returns action-level compliance data with optional filters.</summary>
    Task<IReadOnlyList<ActionComplianceDto>> GetActionComplianceAsync(
        string? department, Guid? policyId, CancellationToken ct);
}
