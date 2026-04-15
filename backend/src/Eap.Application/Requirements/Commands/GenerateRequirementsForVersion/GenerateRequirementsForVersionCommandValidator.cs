using FluentValidation;

namespace Eap.Application.Requirements.Commands.GenerateRequirementsForVersion;

public sealed class GenerateRequirementsForVersionCommandValidator
    : AbstractValidator<GenerateRequirementsForVersionCommand>
{
    public GenerateRequirementsForVersionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
        RuleFor(x => x.CycleReference).MaximumLength(128);
    }
}
