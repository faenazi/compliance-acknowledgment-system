using Eap.Application.Audit.Models;
using Eap.Application.Common.Models;
using MediatR;

namespace Eap.Application.Audit.Queries.ListAuditLogs;

public sealed record ListAuditLogsQuery(
    int Page = 1,
    int PageSize = 20,
    string? ActionType = null,
    string? EntityType = null,
    Guid? ActorUserId = null,
    DateTimeOffset? FromDate = null,
    DateTimeOffset? ToDate = null,
    string? Search = null) : IRequest<PagedResult<AuditLogDto>>;
