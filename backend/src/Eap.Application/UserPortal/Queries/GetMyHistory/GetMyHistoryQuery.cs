using Eap.Application.Common.Models;
using Eap.Application.UserPortal.Models;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMyHistory;

/// <summary>Returns the paginated submission history for the current user.</summary>
public sealed record GetMyHistoryQuery(int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<MyHistoryItemDto>>;
