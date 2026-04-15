using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using MediatR;

namespace Eap.Application.Acknowledgments.Queries.GetAcknowledgmentDefinitionById;

public sealed class GetAcknowledgmentDefinitionByIdQueryHandler
    : IRequestHandler<GetAcknowledgmentDefinitionByIdQuery, AcknowledgmentDefinitionDetailDto>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IMapper _mapper;

    public GetAcknowledgmentDefinitionByIdQueryHandler(
        IAcknowledgmentRepository acknowledgments,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _mapper = mapper;
    }

    public async Task<AcknowledgmentDefinitionDetailDto> Handle(
        GetAcknowledgmentDefinitionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments.FindByIdAsync(request.DefinitionId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        return _mapper.Map<AcknowledgmentDefinitionDetailDto>(definition);
    }
}
