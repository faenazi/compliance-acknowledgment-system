using FluentValidation;

namespace Eap.Application.Policies.Commands.ArchivePolicy;

public sealed class ArchivePolicyCommandValidator : AbstractValidator<ArchivePolicyCommand>
{
    public ArchivePolicyCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
    }
}
