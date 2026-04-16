using Eap.Application.Admin.Abstractions;
using Eap.Application.Admin.Models;
using Eap.Application.Common.Models;
using MediatR;

namespace Eap.Application.Admin.Queries.ListUserRequirements;

internal sealed class ListUserRequirementsQueryHandler
    : IRequestHandler<ListUserRequirementsQuery, PagedResult<AdminRequirementSummaryDto>>
{
    private readonly IAdminRepository _repository;

    public ListUserRequirementsQueryHandler(IAdminRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AdminRequirementSummaryDto>> Handle(
        ListUserRequirementsQuery request, CancellationToken cancellationToken)
    {
        var filter = new AdminRequirementsFilter(
            Page: request.Page,
            PageSize: request.PageSize,
            Status: request.Status,
            AcknowledgmentDefinitionId: request.AcknowledgmentDefinitionId,
            PolicyId: request.PolicyId,
            Department: request.Department,
            RecurrenceModel: request.RecurrenceModel,
            DueDateFrom: request.DueDateFrom,
            DueDateTo: request.DueDateTo,
            Search: request.Search,
            CurrentOnly: request.CurrentOnly);

        var (items, totalCount) = await _repository.ListRequirementsAsync(filter, cancellationToken);

        return new PagedResult<AdminRequirementSummaryDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
