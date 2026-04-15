using Eap.Application.Acknowledgments.Models;
using Eap.Domain.Acknowledgment;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.UpdateAcknowledgmentVersionDraft;

/// <summary>Updates editable fields on a draft acknowledgment version.</summary>
public sealed record UpdateAcknowledgmentVersionDraftCommand(
    Guid DefinitionId,
    Guid VersionId,
    Guid PolicyVersionId,
    ActionType ActionType,
    RecurrenceModel RecurrenceModel,
    string? VersionLabel,
    string? Summary,
    string? CommitmentText,
    DateOnly? StartDate,
    DateOnly? DueDate) : IRequest<AcknowledgmentVersionDetailDto>;
