using Eap.Domain.Acknowledgment;
using Eap.Domain.Requirements;

namespace Eap.Application.Compliance.Models;

/// <summary>Compliance dashboard summary (FR-110 to FR-113, BR-120 to BR-123).</summary>
public sealed class ComplianceDashboardDto
{
    public int TotalRequirements { get; init; }
    public int CompletedRequirements { get; init; }
    public int PendingRequirements { get; init; }
    public int OverdueRequirements { get; init; }
    public decimal CompletionRate { get; init; }

    public IReadOnlyList<DepartmentComplianceDto> ComplianceByDepartment { get; init; } = [];
    public IReadOnlyList<ActionComplianceDto> ComplianceByAction { get; init; } = [];
    public IReadOnlyList<NonCompliantUserSummaryDto> TopNonCompliantUsers { get; init; } = [];
}

/// <summary>Compliance summary grouped by department (FR-113).</summary>
public sealed class DepartmentComplianceDto
{
    public string Department { get; init; } = default!;
    public int TotalAssigned { get; init; }
    public int Completed { get; init; }
    public int Pending { get; init; }
    public int Overdue { get; init; }
    public decimal CompletionRate { get; init; }
}

/// <summary>Compliance summary grouped by acknowledgment action (FR-132).</summary>
public sealed class ActionComplianceDto
{
    public Guid AcknowledgmentDefinitionId { get; init; }
    public string ActionTitle { get; init; } = default!;
    public ActionType ActionType { get; init; }
    public string OwnerDepartment { get; init; } = default!;
    public int TotalAssigned { get; init; }
    public int Completed { get; init; }
    public int Pending { get; init; }
    public int Overdue { get; init; }
    public decimal CompletionRate { get; init; }
}

/// <summary>Summary of a non-compliant user (FR-112).</summary>
public sealed class NonCompliantUserSummaryDto
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = default!;
    public string Department { get; init; } = default!;
    public string? Email { get; init; }
    public int PendingCount { get; init; }
    public int OverdueCount { get; init; }
    public int TotalNonCompliant { get; init; }
}

/// <summary>Detailed non-compliant user row for the compliance report table (FR-131).</summary>
public sealed class NonCompliantUserDetailDto
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = default!;
    public string Department { get; init; } = default!;
    public string? Email { get; init; }

    public Guid RequirementId { get; init; }
    public Guid AcknowledgmentDefinitionId { get; init; }
    public Guid AcknowledgmentVersionId { get; init; }
    public string ActionTitle { get; init; } = default!;
    public ActionType ActionType { get; init; }
    public UserActionRequirementStatus Status { get; init; }
    public DateOnly? DueDate { get; init; }
    public DateTimeOffset AssignedAtUtc { get; init; }
    public string CycleReference { get; init; } = default!;
}

/// <summary>Filter parameters for compliance report queries.</summary>
public sealed record ComplianceReportFilter(
    int Page,
    int PageSize,
    string? Department = null,
    Guid? AcknowledgmentDefinitionId = null,
    Guid? PolicyId = null,
    UserActionRequirementStatus? Status = null,
    string? Search = null);

/// <summary>Filter parameters for the compliance dashboard.</summary>
public sealed record ComplianceDashboardFilter(
    string? Department = null,
    Guid? AcknowledgmentDefinitionId = null,
    Guid? PolicyId = null,
    int TopNonCompliantLimit = 10);
