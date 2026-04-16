using Eap.Application.Compliance.Abstractions;
using Eap.Application.Compliance.Models;
using Eap.Domain.Identity;
using Eap.Domain.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Eap.Api.Features.Compliance;

/// <summary>
/// Export endpoints for compliance reports (Sprint 8, FR-135).
/// Generates CSV files (Excel-compatible) for practical operational exports.
/// </summary>
[ApiController]
[Route("api/admin/compliance/export")]
[Authorize(Roles = SystemRoles.SystemAdministrator + ","
    + SystemRoles.ComplianceViewer + ","
    + SystemRoles.Auditor)]
public sealed class ComplianceExportController : ControllerBase
{
    private readonly IComplianceRepository _complianceRepo;

    public ComplianceExportController(IComplianceRepository complianceRepo)
    {
        _complianceRepo = complianceRepo;
    }

    /// <summary>Export non-compliant users report as CSV.</summary>
    [HttpGet("non-compliant")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportNonCompliantUsers(
        [FromQuery] string? department = null,
        [FromQuery] Guid? acknowledgmentDefinitionId = null,
        [FromQuery] Guid? policyId = null,
        [FromQuery] UserActionRequirementStatus? status = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var filter = new ComplianceReportFilter(
            Page: 1, PageSize: 10_000, // Export up to 10k rows
            Department: department,
            AcknowledgmentDefinitionId: acknowledgmentDefinitionId,
            PolicyId: policyId,
            Status: status,
            Search: search);

        var (items, _) = await _complianceRepo.ListNonCompliantUsersAsync(filter, ct);

        var csv = new StringBuilder();
        // BOM for Excel UTF-8 compatibility
        csv.Append('\uFEFF');
        csv.AppendLine("الاسم,القسم,البريد الإلكتروني,الإجراء,الحالة,تاريخ الاستحقاق,تاريخ التعيين,دورة المرجع");

        foreach (var item in items)
        {
            csv.AppendLine(string.Join(",",
                Escape(item.DisplayName),
                Escape(item.Department),
                Escape(item.Email ?? ""),
                Escape(item.ActionTitle),
                Escape(StatusLabel(item.Status)),
                item.DueDate?.ToString("yyyy-MM-dd") ?? "",
                item.AssignedAtUtc.ToString("yyyy-MM-dd"),
                Escape(item.CycleReference)));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv; charset=utf-8",
            $"non-compliant-users-{DateTime.UtcNow:yyyy-MM-dd}.csv");
    }

    /// <summary>Export department compliance report as CSV.</summary>
    [HttpGet("departments")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportDepartmentCompliance(
        [FromQuery] string? department = null,
        [FromQuery] Guid? acknowledgmentDefinitionId = null,
        [FromQuery] Guid? policyId = null,
        CancellationToken ct = default)
    {
        var items = await _complianceRepo.GetDepartmentComplianceAsync(
            department, acknowledgmentDefinitionId, policyId, ct);

        var csv = new StringBuilder();
        csv.Append('\uFEFF');
        csv.AppendLine("القسم,إجمالي المعيّن,مكتمل,معلّق,متأخر,نسبة الإنجاز");

        foreach (var d in items)
        {
            csv.AppendLine(string.Join(",",
                Escape(d.Department),
                d.TotalAssigned,
                d.Completed,
                d.Pending,
                d.Overdue,
                $"{d.CompletionRate}%"));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv; charset=utf-8",
            $"department-compliance-{DateTime.UtcNow:yyyy-MM-dd}.csv");
    }

    /// <summary>Export action compliance report as CSV.</summary>
    [HttpGet("actions")]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportActionCompliance(
        [FromQuery] string? department = null,
        [FromQuery] Guid? policyId = null,
        CancellationToken ct = default)
    {
        var items = await _complianceRepo.GetActionComplianceAsync(department, policyId, ct);

        var csv = new StringBuilder();
        csv.Append('\uFEFF');
        csv.AppendLine("الإجراء,النوع,القسم المالك,إجمالي المعيّن,مكتمل,معلّق,متأخر,نسبة الإنجاز");

        foreach (var a in items)
        {
            csv.AppendLine(string.Join(",",
                Escape(a.ActionTitle),
                Escape(a.ActionType.ToString()),
                Escape(a.OwnerDepartment),
                a.TotalAssigned,
                a.Completed,
                a.Pending,
                a.Overdue,
                $"{a.CompletionRate}%"));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv; charset=utf-8",
            $"action-compliance-{DateTime.UtcNow:yyyy-MM-dd}.csv");
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static string StatusLabel(UserActionRequirementStatus status) => status switch
    {
        UserActionRequirementStatus.Pending => "معلّق",
        UserActionRequirementStatus.Completed => "مكتمل",
        UserActionRequirementStatus.Overdue => "متأخر",
        UserActionRequirementStatus.Cancelled => "ملغي",
        _ => status.ToString(),
    };
}
