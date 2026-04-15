using Eap.Application.Acknowledgments.Models;
using Eap.Domain.Acknowledgment;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.CreateAcknowledgmentDefinition;

/// <summary>Creates a new master acknowledgment definition (no version yet).</summary>
public sealed record CreateAcknowledgmentDefinitionCommand(
    string Title,
    string OwnerDepartment,
    ActionType DefaultActionType,
    string? Description) : IRequest<AcknowledgmentDefinitionDetailDto>;
