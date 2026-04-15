using Eap.Application.Audience.Models;
using MediatR;

namespace Eap.Application.Audience.Queries.PreviewAudience;

/// <summary>
/// Evaluates the current audience configuration against the directory and returns
/// the estimated size plus a capped sample of matched users.
/// </summary>
public sealed record PreviewAudienceQuery(
    Guid DefinitionId,
    Guid VersionId,
    int SampleSize = 25) : IRequest<AudiencePreviewDto>;
