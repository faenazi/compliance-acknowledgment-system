using Eap.Application.Acknowledgments.Models;
using MediatR;

namespace Eap.Application.Acknowledgments.Queries.GetAcknowledgmentVersionById;

public sealed record GetAcknowledgmentVersionByIdQuery(Guid DefinitionId, Guid VersionId)
    : IRequest<AcknowledgmentVersionDetailDto>;
