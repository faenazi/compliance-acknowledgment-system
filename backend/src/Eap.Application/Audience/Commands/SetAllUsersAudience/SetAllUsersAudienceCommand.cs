using Eap.Application.Audience.Models;
using MediatR;

namespace Eap.Application.Audience.Commands.SetAllUsersAudience;

/// <summary>
/// Convenience command — replaces the inclusion rules with the "all users" rule (BR-051).
/// </summary>
public sealed record SetAllUsersAudienceCommand(
    Guid DefinitionId,
    Guid VersionId) : IRequest<AudienceDefinitionDto>;
