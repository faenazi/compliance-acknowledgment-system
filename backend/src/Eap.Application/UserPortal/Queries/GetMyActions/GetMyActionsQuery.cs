using Eap.Application.Common.Models;
using Eap.Application.UserPortal.Models;
using Eap.Domain.Requirements;
using MediatR;

namespace Eap.Application.UserPortal.Queries.GetMyActions;

/// <summary>Returns a paginated, filterable list of the current user's action requirements.</summary>
public sealed record GetMyActionsQuery(
    int Page = 1,
    int PageSize = 20,
    UserActionRequirementStatus? Status = null,
    string? Search = null) : IRequest<PagedResult<MyActionSummaryDto>>;
