using Eap.Application.Acknowledgments.Models;
using MediatR;

namespace Eap.Application.Acknowledgments.Queries.GetAcknowledgmentDefinitionById;

public sealed record GetAcknowledgmentDefinitionByIdQuery(Guid DefinitionId)
    : IRequest<AcknowledgmentDefinitionDetailDto>;
