using Eap.Application.Acknowledgments.Models;
using Eap.Application.Common.Models;
using Eap.Domain.Acknowledgment;
using MediatR;

namespace Eap.Application.Acknowledgments.Queries.ListAcknowledgmentDefinitions;

/// <summary>Paged list of acknowledgment definitions with optional filters.</summary>
public sealed record ListAcknowledgmentDefinitionsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    AcknowledgmentStatus? Status = null,
    string? OwnerDepartment = null,
    ActionType? ActionType = null) : IRequest<PagedResult<AcknowledgmentDefinitionSummaryDto>>;
