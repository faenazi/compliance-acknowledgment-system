using Eap.Application.Policies.Abstractions;
using Microsoft.Extensions.Logging;

namespace Eap.Infrastructure.Policies.Audit;

/// <summary>
/// Sprint 2 audit sink for policy-management events. Writes structured Serilog
/// events tagged with <c>AuditEvent</c>; a dedicated immutable AuditLog store
/// is introduced with the audit module in a later sprint (BR-130, BR-132)
/// without requiring the application layer to change.
/// </summary>
internal sealed class PolicyAuditLogger : IPolicyAuditLogger
{
    private readonly ILogger<PolicyAuditLogger> _logger;

    public PolicyAuditLogger(ILogger<PolicyAuditLogger> logger)
    {
        _logger = logger;
    }

    public void PolicyCreated(Guid policyId, string policyCode, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {PolicyCode} {Actor}",
            "PolicyCreated", policyId, policyCode, actor ?? "-");

    public void PolicyMetadataUpdated(Guid policyId, string policyCode, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {PolicyCode} {Actor}",
            "PolicyMetadataUpdated", policyId, policyCode, actor ?? "-");

    public void PolicyArchived(Guid policyId, string policyCode, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {PolicyCode} {Actor}",
            "PolicyArchived", policyId, policyCode, actor ?? "-");

    public void PolicyVersionCreated(Guid policyId, Guid versionId, int versionNumber, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {VersionId} {VersionNumber} {Actor}",
            "PolicyVersionCreated", policyId, versionId, versionNumber, actor ?? "-");

    public void PolicyVersionUpdated(Guid policyId, Guid versionId, int versionNumber, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {VersionId} {VersionNumber} {Actor}",
            "PolicyVersionUpdated", policyId, versionId, versionNumber, actor ?? "-");

    public void PolicyVersionPublished(Guid policyId, Guid versionId, int versionNumber, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {VersionId} {VersionNumber} {Actor}",
            "PolicyVersionPublished", policyId, versionId, versionNumber, actor ?? "-");

    public void PolicyVersionArchived(Guid policyId, Guid versionId, int versionNumber, string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {VersionId} {VersionNumber} {Actor}",
            "PolicyVersionArchived", policyId, versionId, versionNumber, actor ?? "-");

    public void PolicyDocumentUploaded(
        Guid policyId,
        Guid versionId,
        Guid documentId,
        string fileName,
        long fileSize,
        string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {VersionId} {DocumentId} {FileName} {FileSize} {Actor}",
            "PolicyDocumentUploaded", policyId, versionId, documentId, fileName, fileSize, actor ?? "-");

    public void PolicyDocumentDownloaded(
        Guid policyId,
        Guid versionId,
        Guid documentId,
        string fileName,
        string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {PolicyId} {VersionId} {DocumentId} {FileName} {Actor}",
            "PolicyDocumentDownloaded", policyId, versionId, documentId, fileName, actor ?? "-");
}
