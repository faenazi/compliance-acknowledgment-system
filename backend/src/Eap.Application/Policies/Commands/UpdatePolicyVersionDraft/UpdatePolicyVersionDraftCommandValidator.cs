using FluentValidation;

namespace Eap.Application.Policies.Commands.UpdatePolicyVersionDraft;

public sealed class UpdatePolicyVersionDraftCommandValidator : AbstractValidator<UpdatePolicyVersionDraftCommand>
{
    public UpdatePolicyVersionDraftCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
        RuleFor(x => x.VersionLabel).MaximumLength(128);
        RuleFor(x => x.Summary).MaximumLength(4000);
    }
}
