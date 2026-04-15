using FluentValidation;

namespace Eap.Application.Policies.Commands.UpdatePolicy;

public sealed class UpdatePolicyCommandValidator : AbstractValidator<UpdatePolicyCommand>
{
    public UpdatePolicyCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(256);

        RuleFor(x => x.OwnerDepartment)
            .NotEmpty().WithMessage("Owner department is required (BR-014).")
            .MaximumLength(256);

        RuleFor(x => x.Category)
            .MaximumLength(128);

        RuleFor(x => x.Description)
            .MaximumLength(4000);
    }
}
