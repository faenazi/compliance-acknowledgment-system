using Eap.Application.Audience.Models;
using MediatR;

namespace Eap.Application.Audience.Queries.GetAudienceByVersion;

/// <summary>Returns the audience configuration attached to an acknowledgment version, if any.</summary>
public sealed record GetAudienceByVersionQuery(
    Guid DefinitionId,
    Guid VersionId) : IRequest<AudienceDefinitionDto?>;
