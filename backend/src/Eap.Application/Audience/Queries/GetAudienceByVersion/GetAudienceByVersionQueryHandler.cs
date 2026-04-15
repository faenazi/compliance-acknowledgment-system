using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Audience.Models;
using Eap.Application.Common.Exceptions;
using MediatR;

namespace Eap.Application.Audience.Queries.GetAudienceByVersion;

public sealed class GetAudienceByVersionQueryHandler
    : IRequestHandler<GetAudienceByVersionQuery, AudienceDefinitionDto?>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IMapper _mapper;

    public GetAudienceByVersionQueryHandler(
        IAcknowledgmentRepository acknowledgments,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _mapper = mapper;
    }

    public async Task<AudienceDefinitionDto?> Handle(
        GetAudienceByVersionQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindByIdAsync(request.DefinitionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        return version.Audience is null
            ? null
            : _mapper.Map<AudienceDefinitionDto>(version.Audience);
    }
}
