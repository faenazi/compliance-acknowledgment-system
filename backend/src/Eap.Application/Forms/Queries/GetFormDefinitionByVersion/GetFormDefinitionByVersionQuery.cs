using Eap.Application.Forms.Models;
using MediatR;

namespace Eap.Application.Forms.Queries.GetFormDefinitionByVersion;

public sealed record GetFormDefinitionByVersionQuery(
    Guid DefinitionId,
    Guid VersionId) : IRequest<FormDefinitionDto?>;
