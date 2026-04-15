using Eap.Application.Audience.Abstractions;
using Microsoft.Extensions.Logging;

namespace Eap.Infrastructure.Audience.Audit;

internal sealed class AudienceAuditLogger : IAudienceAuditLogger
{
    private readonly ILogger<AudienceAuditLogger> _logger;

    public AudienceAuditLogger(ILogger<AudienceAuditLogger> logger)
    {
        _logger = logger;
    }

    public void AudienceConfigured(
        Guid definitionId,
        Guid versionId,
        Guid audienceId,
        int inclusionCount,
        int exclusionCount,
        string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {VersionId} {AudienceId} {InclusionCount} {ExclusionCount} {Actor}",
            "AudienceConfigured", definitionId, versionId, audienceId, inclusionCount, exclusionCount, actor ?? "-");

    public void InclusionRulesReplaced(
        Guid definitionId,
        Guid versionId,
        Guid audienceId,
        int ruleCount,
        string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {VersionId} {AudienceId} {RuleCount} {Actor}",
            "AudienceInclusionRulesReplaced", definitionId, versionId, audienceId, ruleCount, actor ?? "-");

    public void ExclusionRulesReplaced(
        Guid definitionId,
        Guid versionId,
        Guid audienceId,
        int ruleCount,
        string? actor) =>
        _logger.LogInformation(
            "{AuditEvent} {DefinitionId} {VersionId} {AudienceId} {RuleCount} {Actor}",
            "AudienceExclusionRulesReplaced", definitionId, versionId, audienceId, ruleCount, actor ?? "-");
}
