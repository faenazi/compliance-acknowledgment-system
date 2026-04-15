using Eap.Application.Audience.Models;
using MediatR;

namespace Eap.Application.Audience.Commands.ConfigureAudienceExclusions;

/// <summary>
/// Replaces the exclusion rules on a draft version's audience (BR-054 / BR-055).
/// Inclusion rules are preserved.
/// </summary>
public sealed record ConfigureAudienceExclusionsCommand(
    Guid DefinitionId,
    Guid VersionId,
    IReadOnlyList<AudienceRuleInputDto> Rules) : IRequest<AudienceDefinitionDto>;
