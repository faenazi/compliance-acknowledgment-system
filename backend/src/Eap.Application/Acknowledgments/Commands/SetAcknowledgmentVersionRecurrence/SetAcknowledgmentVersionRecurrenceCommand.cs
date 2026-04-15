using Eap.Application.Acknowledgments.Models;
using Eap.Domain.Acknowledgment;
using MediatR;

namespace Eap.Application.Acknowledgments.Commands.SetAcknowledgmentVersionRecurrence;

/// <summary>
/// Dedicated endpoint for the Recurrence Configuration admin page (BR-040/BR-046).
/// Updates the recurrence model and the start/due date window on a draft version
/// without re-posting every metadata field.
/// </summary>
public sealed record SetAcknowledgmentVersionRecurrenceCommand(
    Guid DefinitionId,
    Guid VersionId,
    RecurrenceModel RecurrenceModel,
    DateOnly? StartDate,
    DateOnly? DueDate) : IRequest<AcknowledgmentVersionDetailDto>;
