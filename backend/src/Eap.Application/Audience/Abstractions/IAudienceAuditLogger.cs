namespace Eap.Application.Audience.Abstractions;

/// <summary>
/// Audit surface for audience-targeting operations. Writes Serilog events tagged
/// with <c>AuditEvent</c>. Replaced by the durable AuditLog store in a later sprint.
/// </summary>
public interface IAudienceAuditLogger
{
    void AudienceConfigured(Guid definitionId, Guid versionId, Guid audienceId, int inclusionCount, int exclusionCount, string? actor);

    void InclusionRulesReplaced(Guid definitionId, Guid versionId, Guid audienceId, int ruleCount, string? actor);

    void ExclusionRulesReplaced(Guid definitionId, Guid versionId, Guid audienceId, int ruleCount, string? actor);
}
