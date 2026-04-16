using Eap.Application.Audit.Abstractions;
using Eap.Application.Audit.Models;
using Eap.Application.Common.Models;
using MediatR;

namespace Eap.Application.Audit.Queries.ListAuditLogs;

internal sealed class ListAuditLogsQueryHandler
    : IRequestHandler<ListAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _repository;

    public ListAuditLogsQueryHandler(IAuditLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(
        ListAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var filter = new AuditLogFilter(
            Page: request.Page,
            PageSize: request.PageSize,
            ActionType: request.ActionType,
            EntityType: request.EntityType,
            ActorUserId: request.ActorUserId,
            FromDate: request.FromDate,
            ToDate: request.ToDate,
            Search: request.Search);

        var (items, totalCount) = await _repository.ListAsync(filter, cancellationToken);

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}
