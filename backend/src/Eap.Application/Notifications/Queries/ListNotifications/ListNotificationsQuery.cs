using Eap.Application.Common.Models;
using Eap.Application.Notifications.Models;
using Eap.Domain.Notifications;
using MediatR;

namespace Eap.Application.Notifications.Queries.ListNotifications;

public sealed record ListNotificationsQuery(
    int Page = 1,
    int PageSize = 20,
    NotificationType? Type = null,
    NotificationStatus? Status = null,
    string? Search = null) : IRequest<PagedResult<NotificationSummaryDto>>;
