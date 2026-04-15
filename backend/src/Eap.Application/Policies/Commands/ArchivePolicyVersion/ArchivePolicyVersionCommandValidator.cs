using FluentValidation;

namespace Eap.Application.Policies.Commands.ArchivePolicyVersion;

public sealed class ArchivePolicyVersionCommandValidator : AbstractValidator<ArchivePolicyVersionCommand>
{
    public ArchivePolicyVersionCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
    }
}
