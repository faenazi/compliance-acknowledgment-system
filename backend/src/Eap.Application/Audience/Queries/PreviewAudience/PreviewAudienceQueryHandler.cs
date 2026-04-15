using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Audience.Abstractions;
using Eap.Application.Audience.Models;
using Eap.Application.Common.Exceptions;
using MediatR;

namespace Eap.Application.Audience.Queries.PreviewAudience;

public sealed class PreviewAudienceQueryHandler
    : IRequestHandler<PreviewAudienceQuery, AudiencePreviewDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAudienceResolver _resolver;

    public PreviewAudienceQueryHandler(
        IAcknowledgmentRepository acknowledgments,
        IAudienceResolver resolver)
    {
        _acknowledgments = acknowledgments;
        _resolver = resolver;
    }

    public async Task<AudiencePreviewDto> Handle(
        PreviewAudienceQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        if (version.Audience is null)
        {
            return new AudiencePreviewDto
            {
                EstimatedUserCount = 0,
                InclusionMatchedCount = 0,
                ExclusionMatchedCount = 0,
                SampleUsers = Array.Empty<AudiencePreviewUserDto>(),
            };
        }

        var options = new AudienceResolutionOptions(
            SampleSize: Math.Clamp(request.SampleSize, 0, 100),
            IncludeUserIds: false);

        var result = await _resolver
            .ResolveAsync(version.Audience, options, cancellationToken)
            .ConfigureAwait(false);

        return new AudiencePreviewDto
        {
            EstimatedUserCount = result.MatchedUserIds.Count,
            InclusionMatchedCount = result.InclusionMatchedCount,
            ExclusionMatchedCount = result.ExclusionMatchedCount,
            SampleUsers = result.SampleUsers
                .Select(u => new AudiencePreviewUserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    DisplayName = u.DisplayName,
                    Department = u.Department,
                })
                .ToList(),
        };
    }
}
