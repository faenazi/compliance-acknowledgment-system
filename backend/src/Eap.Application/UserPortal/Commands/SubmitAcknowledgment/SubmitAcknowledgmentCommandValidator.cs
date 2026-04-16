using FluentValidation;

namespace Eap.Application.UserPortal.Commands.SubmitAcknowledgment;

public sealed class SubmitAcknowledgmentCommandValidator : AbstractValidator<SubmitAcknowledgmentCommand>
{
    public SubmitAcknowledgmentCommandValidator()
    {
        RuleFor(x => x.RequirementId)
            .NotEmpty()
            .WithMessage("Requirement id is required.");
    }
}
