using Eap.Application.Acknowledgments.Models;
using Eap.Domain.Acknowledgment;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.UpdateAcknowledgmentDefinition;

/// <summary>Updates mutable master-metadata fields on an existing acknowledgment definition.</summary>
public sealed record UpdateAcknowledgmentDefinitionCommand(
    Guid DefinitionId,
    string Title,
    string OwnerDepartment,
    ActionType DefaultActionType,
    string? Description) : IRequest<AcknowledgmentDefinitionDetailDto>;
