using Eap.Application.Acknowledgments.Models;
using MediatR;

namespace Eap.Application.Acknowledgments.Queries.ListAcknowledgmentVersions;

public sealed record ListAcknowledgmentVersionsQuery(Guid DefinitionId)
    : IRequest<IReadOnlyList<AcknowledgmentVersionSummaryDto>>;
