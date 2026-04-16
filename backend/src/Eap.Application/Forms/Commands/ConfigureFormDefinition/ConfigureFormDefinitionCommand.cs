using Eap.Application.Forms.Models;
using MediatR;

namespace Eap.Application.Forms.Commands.ConfigureFormDefinition;

/// <summary>
/// Replaces all fields on a draft acknowledgment version's form definition
/// (BR-070..BR-073). Creates the form definition if it does not yet exist.
/// </summary>
public sealed record ConfigureFormDefinitionCommand(
    Guid DefinitionId,
    Guid VersionId,
    IReadOnlyList<FormFieldInputDto> Fields) : IRequest<FormDefinitionDto>;
