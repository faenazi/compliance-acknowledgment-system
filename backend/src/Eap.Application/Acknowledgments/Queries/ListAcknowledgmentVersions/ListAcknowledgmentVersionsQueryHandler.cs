using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Exceptions;
using MediatR;

namespace Eap.Application.Acknowledgments.Queries.ListAcknowledgmentVersions;

public sealed class ListAcknowledgmentVersionsQueryHandler
    : IRequestHandler<ListAcknowledgmentVersionsQuery, IReadOnlyList<AcknowledgmentVersionSummaryDto>>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IMapper _mapper;

    public ListAcknowledgmentVersionsQueryHandler(
        IAcknowledgmentRepository acknowledgments,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AcknowledgmentVersionSummaryDto>> Handle(
        ListAcknowledgmentVersionsQuery request,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments.FindByIdAsync(request.DefinitionId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentDefinition", request.DefinitionId);

        return _mapper.Map<IReadOnlyList<AcknowledgmentVersionSummaryDto>>(
            definition.Versions.OrderByDescending(v => v.VersionNumber).ToList());
    }
}
