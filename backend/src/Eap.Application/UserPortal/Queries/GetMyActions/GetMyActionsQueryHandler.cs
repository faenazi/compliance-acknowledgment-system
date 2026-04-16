using Eap.Application.Common.Models;
using Eap.Application.Identity.Abstractions;
using Eap.Application.UserPortal.Abstractions;
using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMyActions;

public sealed class GetMyActionsQueryHandler
    : IRequestHandler<GetMyActionsQuery, PagedResult<MyActionSummaryDto>>
{
    private readonly IUserPortalRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetMyActionsQueryHandler(
        IUserPortalRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<MyActionSummaryDto>> Handle(
        GetMyActionsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new InvalidOperationException("Authenticated user id is required.");

        var filter = new MyActionsFilter(request.Page, request.PageSize, request.Status, request.Search);
        var (items, totalCount) = await _repository.ListActionsAsync(userId, filter, cancellationToken);

        return new PagedResult<MyActionSummaryDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
