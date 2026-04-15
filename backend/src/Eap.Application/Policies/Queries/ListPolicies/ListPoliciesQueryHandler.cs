using AutoMapper;
using Eap.Application.Common.Models;
using Eap.Application.Policies.Abstractions;
using Eap.Application.Policies.Models;
using MediatR;

namespace Eap.Application.Policies.Queries.ListPolicies;

public sealed class ListPoliciesQueryHandler
    : IRequestHandler<ListPoliciesQuery, PagedResult<PolicySummaryDto>>
{
    private readonly IPolicyRepository _policies;
    private readonly IMapper _mapper;

    public ListPoliciesQueryHandler(IPolicyRepository policies, IMapper mapper)
    {
        _policies = policies;
        _mapper = mapper;
    }

    public async Task<PagedResult<PolicySummaryDto>> Handle(
        ListPoliciesQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new PolicyListFilter(
            Page: request.Page,
            PageSize: request.PageSize,
            Search: string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim(),
            Status: request.Status,
            OwnerDepartment: string.IsNullOrWhiteSpace(request.OwnerDepartment) ? null : request.OwnerDepartment.Trim(),
            Category: string.IsNullOrWhiteSpace(request.Category) ? null : request.Category.Trim());

        var (items, totalCount) = await _policies.ListAsync(filter, cancellationToken).ConfigureAwait(false);

        return new PagedResult<PolicySummaryDto>
        {
            Items = _mapper.Map<IReadOnlyList<PolicySummaryDto>>(items),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
