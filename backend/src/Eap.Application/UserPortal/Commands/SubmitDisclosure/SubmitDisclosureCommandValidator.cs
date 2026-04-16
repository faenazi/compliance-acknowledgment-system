using FluentValidation;

namespace Eap.Application.UserPortal.Commands.SubmitDisclosure;

public sealed class SubmitDisclosureCommandValidator : AbstractValidator<SubmitDisclosureCommand>
{
    public SubmitDisclosureCommandValidator()
    {
        RuleFor(x => x.RequirementId)
            .NotEmpty()
            .WithMessage("Requirement id is required.");

        RuleFor(x => x.SubmissionJson)
            .NotEmpty()
            .WithMessage("Submission data is required.");
    }
}
