using FluentValidation;

namespace Eap.Application.Acknowledgments.Commands.SetAcknowledgmentVersionRecurrence;

public sealed class SetAcknowledgmentVersionRecurrenceCommandValidator
    : AbstractValidator<SetAcknowledgmentVersionRecurrenceCommand>
{
    public SetAcknowledgmentVersionRecurrenceCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();

        RuleFor(x => x.RecurrenceModel)
            .IsInEnum().WithMessage("Recurrence model is invalid.");

        RuleFor(x => x)
            .Must(x => x.DueDate is null || x.StartDate is null || x.DueDate >= x.StartDate)
            .WithName(nameof(SetAcknowledgmentVersionRecurrenceCommand.DueDate))
            .WithMessage("Due date cannot be earlier than start date.");
    }
}
