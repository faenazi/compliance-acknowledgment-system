using Eap.Application.Forms.Models;
using MediatR;

namespace Eap.Application.Forms.Queries.ListSubmissionsByVersion;

public sealed record ListSubmissionsByVersionQuery(
    Guid DefinitionId,
    Guid VersionId,
    int Page = 1,
    int PageSize = 25) : IRequest<ListSubmissionsByVersionResult>;

public sealed record ListSubmissionsByVersionResult(
    IReadOnlyList<UserSubmissionSummaryDto> Items,
    int TotalCount);
