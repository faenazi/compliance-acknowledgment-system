using Eap.Domain.Acknowledgment;
using FluentValidation;

namespace Eap.Application.Acknowledgments.Commands.CreateAcknowledgmentVersion;

public sealed class CreateAcknowledgmentVersionCommandValidator
    : AbstractValidator<CreateAcknowledgmentVersionCommand>
{
    public CreateAcknowledgmentVersionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();

        RuleFor(x => x.PolicyVersionId)
            .NotEmpty().WithMessage("A linked policy version is required (LR-001).");

        RuleFor(x => x.ActionType)
            .IsInEnum().WithMessage("Action type is invalid.");

        RuleFor(x => x.RecurrenceModel)
            .IsInEnum().WithMessage("Recurrence model is invalid.");

        RuleFor(x => x.VersionLabel).MaximumLength(64);
        RuleFor(x => x.Summary).MaximumLength(4000);
        RuleFor(x => x.CommitmentText).MaximumLength(4000);

        RuleFor(x => x)
            .Must(x => x.DueDate is null || x.StartDate is null || x.DueDate >= x.StartDate)
            .WithName(nameof(CreateAcknowledgmentVersionCommand.DueDate))
            .WithMessage("Due date cannot be earlier than start date.");

        When(x => x.ActionType == ActionType.AcknowledgmentWithCommitment, () =>
        {
            RuleFor(x => x.CommitmentText)
                .NotEmpty()
                .WithMessage("Commitment text is required for 'Acknowledgment with Commitment' action type.");
        });
    }
}
