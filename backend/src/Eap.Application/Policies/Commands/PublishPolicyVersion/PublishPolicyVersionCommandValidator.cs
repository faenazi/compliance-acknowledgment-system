using FluentValidation;

namespace Eap.Application.Policies.Commands.PublishPolicyVersion;

public sealed class PublishPolicyVersionCommandValidator : AbstractValidator<PublishPolicyVersionCommand>
{
    public PublishPolicyVersionCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
    }
}
