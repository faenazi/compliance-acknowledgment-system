using FluentValidation;

namespace Eap.Application.Policies.Commands.CreatePolicyVersion;

public sealed class CreatePolicyVersionCommandValidator : AbstractValidator<CreatePolicyVersionCommand>
{
    public CreatePolicyVersionCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();

        RuleFor(x => x.VersionLabel).MaximumLength(128);
        RuleFor(x => x.Summary).MaximumLength(4000);
    }
}
