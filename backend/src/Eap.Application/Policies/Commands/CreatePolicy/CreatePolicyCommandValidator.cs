using FluentValidation;

namespace Eap.Application.Policies.Commands.CreatePolicy;

public sealed class CreatePolicyCommandValidator : AbstractValidator<CreatePolicyCommand>
{
    public CreatePolicyCommandValidator()
    {
        RuleFor(x => x.PolicyCode)
            .NotEmpty().WithMessage("Policy code is required.")
            .MaximumLength(64)
            .Matches("^[A-Za-z0-9._-]+$")
            .WithMessage("Policy code may only contain letters, digits, '.', '_' and '-'.");

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
