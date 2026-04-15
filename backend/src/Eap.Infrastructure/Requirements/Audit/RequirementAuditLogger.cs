using Eap.Application.Requirements.Abstractions;
using Microsoft.Extensions.Logging;

namespace Eap.Infrastructure.Requirements.Audit;

internal sealed class RequirementAuditLogger : IRequirementAuditLogger
{
    private readonly ILogger<RequirementAuditLogger> _logger;

    public RequirementAuditLogger(ILogger<RequirementAuditLogger> logger)
    {
        _logger = logger;
    }

    public void RequirementsGenerated(
        Guid acknowledgmentVersionId,
        string cycleReference,
        int createdCount,
        int skippedCount,
        int cancelledCount,
        string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {VersionId} {Cycle} {Created} {Skipped} {Cancelled} {Actor}",
            "UserActionRequirementsGenerated",
            acknowledgmentVersionId, cycleReference, createdCount, skippedCount, cancelledCount, actor ?? "-");
}
