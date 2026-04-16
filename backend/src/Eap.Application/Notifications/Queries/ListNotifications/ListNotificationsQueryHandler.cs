using Eap.Application.Common.Models;
using Eap.Application.Notifications.Abstractions;
using Eap.Application.Notifications.Models;
using MediatR;

namespace Eap.Application.Notifications.Queries.ListNotifications;

internal sealed class ListNotificationsQueryHandler
    : IRequestHandler<ListNotificationsQuery, PagedResult<NotificationSummaryDto>>
{
    private readonly INotificationRepository _repository;

    public ListNotificationsQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<NotificationSummaryDto>> Handle(
        ListNotificationsQuery request, CancellationToken cancellationToken)
    {
        var filter = new NotificationListFilter(
            Page: request.Page,
            PageSize: request.PageSize,
            Type: request.Type,
            Status: request.Status,
            Search: request.Search);

        var (items, totalCount) = await _repository.ListAsync(filter, cancellationToken);

        return new PagedResult<NotificationSummaryDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
