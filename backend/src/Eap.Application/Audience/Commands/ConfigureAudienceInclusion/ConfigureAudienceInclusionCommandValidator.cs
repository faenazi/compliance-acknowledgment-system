using Eap.Domain.Audience;
using FluentValidation;

namespace Eap.Application.Audience.Commands.ConfigureAudienceInclusion;

public sealed class ConfigureAudienceInclusionCommandValidator
    : AbstractValidator<ConfigureAudienceInclusionCommand>
{
    public ConfigureAudienceInclusionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();

        RuleFor(x => x.Rules)
            .NotNull()
            .WithMessage("Inclusion rules payload is required.");

        RuleForEach(x => x.Rules).ChildRules(rule =>
        {
            rule.RuleFor(r => r.RuleType)
                .IsInEnum()
                .WithMessage("Rule type is invalid.");

            rule.RuleFor(r => r.RuleValue)
                .NotEmpty()
                .When(r => r.RuleType != AudienceRuleType.AllUsers)
                .WithMessage("A value is required for Department, AD Group and User rules.");

            rule.RuleFor(r => r.RuleValue)
                .MaximumLength(256);
        });
    }
}
