using Eap.Application.Acknowledgments.Models;
using Eap.Domain.Acknowledgment;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.CreateAcknowledgmentVersion;

/// <summary>Creates a new draft version for the given acknowledgment definition.</summary>
public sealed record CreateAcknowledgmentVersionCommand(
    Guid DefinitionId,
    Guid PolicyVersionId,
    ActionType ActionType,
    RecurrenceModel RecurrenceModel,
    string? VersionLabel,
    string? Summary,
    string? CommitmentText,
    DateOnly? StartDate,
    DateOnly? DueDate) : IRequest<AcknowledgmentVersionDetailDto>;
