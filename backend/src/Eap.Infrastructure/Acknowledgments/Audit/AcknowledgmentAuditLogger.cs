using Eap.Application.Acknowledgments.Abstractions;
using Microsoft.Extensions.Logging;

namespace Eap.Infrastructure.Acknowledgments.Audit;

/// <summary>
/// Sprint 3 audit sink for acknowledgment-management events. Writes structured
/// Serilog events tagged with <c>AuditEvent</c>; a dedicated immutable AuditLog
/// store (BR-130/132) supersedes this sink in a later sprint.
/// </summary>
internal sealed class AcknowledgmentAuditLogger : IAcknowledgmentAuditLogger
{
    private readonly ILogger<AcknowledgmentAuditLogger> _logger;

    public AcknowledgmentAuditLogger(ILogger<AcknowledgmentAuditLogger> logger)
    {
        _logger = logger;
    }

    public void DefinitionCreated(Guid definitionId, string title, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {Title} {Actor}",
            "AcknowledgmentDefinitionCreated", definitionId, title, actor ?? "-");

    public void DefinitionMetadataUpdated(Guid definitionId, string title, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {Title} {Actor}",
            "AcknowledgmentDefinitionUpdated", definitionId, title, actor ?? "-");

    public void DefinitionArchived(Guid definitionId, string title, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {Title} {Actor}",
            "AcknowledgmentDefinitionArchived", definitionId, title, actor ?? "-");

    public void VersionCreated(
        Guid definitionId,
        Guid versionId,
        int versionNumber,
        Guid policyVersionId,
        string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {VersionId} {VersionNumber} {PolicyVersionId} {Actor}",
            "AcknowledgmentVersionCreated", definitionId, versionId, versionNumber, policyVersionId, actor ?? "-");

    public void VersionUpdated(Guid definitionId, Guid versionId, int versionNumber, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {VersionId} {VersionNumber} {Actor}",
            "AcknowledgmentVersionUpdated", definitionId, versionId, versionNumber, actor ?? "-");

    public void VersionPublished(Guid definitionId, Guid versionId, int versionNumber, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {VersionId} {VersionNumber} {Actor}",
            "AcknowledgmentVersionPublished", definitionId, versionId, versionNumber, actor ?? "-");

    public void VersionArchived(Guid definitionId, Guid versionId, int versionNumber, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {VersionId} {VersionNumber} {Actor}",
            "AcknowledgmentVersionArchived", definitionId, versionId, versionNumber, actor ?? "-");
}
