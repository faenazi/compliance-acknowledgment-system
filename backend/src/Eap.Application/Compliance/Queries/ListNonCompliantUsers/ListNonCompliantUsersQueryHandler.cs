using Eap.Application.Common.Models;
using Eap.Application.Compliance.Abstractions;
using Eap.Application.Compliance.Models;
using MediatR;

namespace Eap.Application.Compliance.Queries.ListNonCompliantUsers;

internal sealed class ListNonCompliantUsersQueryHandler
    : IRequestHandler<ListNonCompliantUsersQuery, PagedResult<NonCompliantUserDetailDto>>
{
    private readonly IComplianceRepository _repository;

    public ListNonCompliantUsersQueryHandler(IComplianceRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<NonCompliantUserDetailDto>> Handle(
        ListNonCompliantUsersQuery request, CancellationToken cancellationToken)
    {
        var filter = new ComplianceReportFilter(
            Page: request.Page,
            PageSize: request.PageSize,
            Department: request.Department,
            AcknowledgmentDefinitionId: request.AcknowledgmentDefinitionId,
            PolicyId: request.PolicyId,
            Status: request.Status,
            Search: request.Search);

        var (items, totalCount) = await _repository
            .ListNonCompliantUsersAsync(filter, cancellationToken);

        return new PagedResult<NonCompliantUserDetailDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
