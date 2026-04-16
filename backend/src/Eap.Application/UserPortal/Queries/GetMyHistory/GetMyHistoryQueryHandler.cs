using Eap.Application.Common.Models;
using Eap.Application.Identity.Abstractions;
using Eap.Application.UserPortal.Abstractions;
using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMyHistory;

public sealed class GetMyHistoryQueryHandler
    : IRequestHandler<GetMyHistoryQuery, PagedResult<MyHistoryItemDto>>
{
    private readonly IUserPortalRepository _repository;
    private readonly ICurrentUser _currentUser;

    public GetMyHistoryQueryHandler(
        IUserPortalRepository repository,
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<MyHistoryItemDto>> Handle(
        GetMyHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new InvalidOperationException("Authenticated user id is required.");

        var (items, totalCount) = await _repository.ListHistoryAsync(
            userId, request.Page, request.PageSize, cancellationToken);

        return new PagedResult<MyHistoryItemDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
