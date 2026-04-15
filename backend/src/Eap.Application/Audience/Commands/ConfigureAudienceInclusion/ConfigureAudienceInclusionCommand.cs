using Eap.Application.Audience.Models;
using MediatR;

namespace Eap.Application.Audience.Commands.ConfigureAudienceInclusion;

/// <summary>
/// Replaces the inclusion rules on a draft acknowledgment version's audience
/// (BR-050..BR-053). Existing exclusion rules are preserved.
/// </summary>
public sealed record ConfigureAudienceInclusionCommand(
    Guid DefinitionId,
    Guid VersionId,
    IReadOnlyList<AudienceRuleInputDto> Rules) : IRequest<AudienceDefinitionDto>;
