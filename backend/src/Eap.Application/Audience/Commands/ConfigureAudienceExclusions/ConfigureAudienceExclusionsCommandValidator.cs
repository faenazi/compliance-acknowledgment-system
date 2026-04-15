using Eap.Domain.Audience;
using FluentValidation;

namespace Eap.Application.Audience.Commands.ConfigureAudienceExclusions;

public sealed class ConfigureAudienceExclusionsCommandValidator
    : AbstractValidator<ConfigureAudienceExclusionsCommand>
{
    public ConfigureAudienceExclusionsCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();

        RuleFor(x => x.Rules)
            .NotNull()
            .WithMessage("Exclusion rules payload is required.");

        RuleForEach(x => x.Rules).ChildRules(rule =>
        {
            rule.RuleFor(r => r.RuleType)
                .IsInEnum()
                .Must(t => t != AudienceRuleType.AllUsers)
                .WithMessage("An 'All Users' rule cannot be used as an exclusion (BR-055).");

            rule.RuleFor(r => r.RuleValue)
                .NotEmpty()
                .WithMessage("Exclusion rules require a value.")
                .MaximumLength(256);
        });
    }
}
