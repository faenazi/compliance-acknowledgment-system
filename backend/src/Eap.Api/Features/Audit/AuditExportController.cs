using Eap.Application.Audit.Abstractions;
using Eap.Application.Audit.Models;
using Eap.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Eap.Api.Features.Audit;

/// <summary>
/// Export endpoint for audit log data (Sprint 8, FR-135).
/// </summary>
[ApiController]
[Route("api/admin/audit/export")]
[Authorize(Roles = SystemRoles.SystemAdministrator + ","
    + SystemRoles.Auditor)]
public sealed class AuditExportController : ControllerBase
{
    private readonly IAuditLogRepository _auditRepo;

    public AuditExportController(IAuditLogRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    /// <summary>Export audit log as CSV.</summary>
    [HttpGet]
    [Produces("text/csv")]
    public async Task<IActionResult> ExportAuditLog(
        [FromQuery] string? actionType = null,
        [FromQuery] string? entityType = null,
        [FromQuery] Guid? actorUserId = null,
        [FromQuery] DateTimeOffset? fromDate = null,
        [FromQuery] DateTimeOffset? toDate = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var filter = new AuditLogFilter(
            Page: 1, PageSize: 10_000,
            ActionType: actionType,
            EntityType: entityType,
            ActorUserId: actorUserId,
            FromDate: fromDate,
            ToDate: toDate,
            Search: search);

        var (items, _) = await _auditRepo.ListAsync(filter, ct);

        var csv = new StringBuilder();
        csv.Append('\uFEFF');
        csv.AppendLine("التاريخ والوقت,المستخدم,نوع الإجراء,نوع الكيان,معرّف الكيان,معرّف النسخة,الوصف");

        foreach (var log in items)
        {
            csv.AppendLine(string.Join(",",
                log.ActionTimestampUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                Escape(log.ActorUsername ?? "النظام"),
                Escape(log.ActionType),
                Escape(log.EntityType),
                log.EntityId?.ToString() ?? "",
                log.EntityVersionId?.ToString() ?? "",
                Escape(log.Description ?? "")));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        return File(bytes, "text/csv; charset=utf-8",
            $"audit-log-{DateTime.UtcNow:yyyy-MM-dd}.csv");
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
