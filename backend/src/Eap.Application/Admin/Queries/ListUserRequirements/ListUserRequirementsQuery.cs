using Eap.Application.Admin.Models;
using Eap.Application.Common.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Requirements;
using MediatR;

namespace Eap.Application.Admin.Queries.ListUserRequirements;

/// <summary>Returns a paginated, filtered list of user action requirements for admin monitoring.</summary>
public sealed record ListUserRequirementsQuery(
    int Page,
    int PageSize,
    UserActionRequirementStatus? Status = null,
    Guid? AcknowledgmentDefinitionId = null,
    Guid? PolicyId = null,
    string? Department = null,
    RecurrenceModel? RecurrenceModel = null,
    DateOnly? DueDateFrom = null,
    DateOnly? DueDateTo = null,
    string? Search = null,
    bool CurrentOnly = true) : IRequest<PagedResult<AdminRequirementSummaryDto>>;
