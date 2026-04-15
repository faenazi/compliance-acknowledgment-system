using AutoMapper;
using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Models;
using MediatR;

namespace Eap.Application.Acknowledgments.Queries.ListAcknowledgmentDefinitions;

public sealed class ListAcknowledgmentDefinitionsQueryHandler
    : IRequestHandler<ListAcknowledgmentDefinitionsQuery, PagedResult<AcknowledgmentDefinitionSummaryDto>>
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IMapper _mapper;

    public ListAcknowledgmentDefinitionsQueryHandler(
        IAcknowledgmentRepository acknowledgments,
        IMapper mapper)
    {
        _acknowledgments = acknowledgments;
        _mapper = mapper;
    }

    public async Task<PagedResult<AcknowledgmentDefinitionSummaryDto>> Handle(
        ListAcknowledgmentDefinitionsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new AcknowledgmentListFilter(
            Page: request.Page,
            PageSize: request.PageSize,
            Search: string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim(),
            Status: request.Status,
            OwnerDepartment: string.IsNullOrWhiteSpace(request.OwnerDepartment) ? null : request.OwnerDepartment.Trim(),
            ActionType: request.ActionType);

        var (items, totalCount) = await _acknowledgments.ListAsync(filter, cancellationToken).ConfigureAwait(false);

        return new PagedResult<AcknowledgmentDefinitionSummaryDto>
        {
            Items = _mapper.Map<IReadOnlyList<AcknowledgmentDefinitionSummaryDto>>(items),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
