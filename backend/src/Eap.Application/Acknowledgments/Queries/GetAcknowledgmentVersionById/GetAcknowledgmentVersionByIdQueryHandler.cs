using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using MediatR;

namespace Eap.Application.Acknowledgments.Queries.GetAcknowledgmentVersionById;

public sealed class GetAcknowledgmentVersionByIdQueryHandler
    : IRequestHandler<GetAcknowledgmentVersionByIdQuery, AcknowledgmentVersionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IMapper _mapper;

    public GetAcknowledgmentVersionByIdQueryHandler(
        IAcknowledgmentRepository acknowledgments,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _mapper = mapper;
    }

    public async Task<AcknowledgmentVersionDetailDto> Handle(
        GetAcknowledgmentVersionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments.FindByIdAsync(request.DefinitionId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        var version = definition.Versions.SingleOrDefault(v => v.Id == request.VersionId)
            ?? throw new NotFoundException("AcknowledgmentVersion", request.VersionId);

        return _mapper.Map<AcknowledgmentVersionDetailDto>(version);
    }
}
