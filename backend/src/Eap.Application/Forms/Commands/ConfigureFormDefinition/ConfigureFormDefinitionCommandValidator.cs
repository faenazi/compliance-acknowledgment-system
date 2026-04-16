using Eap.Domain.Forms;
using FluentValidation;

namespace Eap.Application.Forms.Commands.ConfigureFormDefinition;

public sealed class ConfigureFormDefinitionCommandValidator
    : AbstractValidator<ConfigureFormDefinitionCommand>
{
    public ConfigureFormDefinitionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();

        RuleFor(x => x.Fields)
            .NotNull()
            .WithMessage("Fields payload is required.");

        RuleForEach(x => x.Fields).ChildRules(field =>
        {
            field.RuleFor(f => f.FieldKey)
                .NotEmpty()
                .MaximumLength(128)
                .Matches("^[a-zA-Z_][a-zA-Z0-9_]*$")
                .WithMessage("Field key must be a valid identifier (letters, digits, underscores; must start with a letter or underscore).");

            field.RuleFor(f => f.Label)
                .NotEmpty()
                .MaximumLength(500);

            field.RuleFor(f => f.FieldType)
                .IsInEnum()
                .WithMessage("Invalid field type.");

            field.RuleFor(f => f.HelpText).MaximumLength(1000);
            field.RuleFor(f => f.Placeholder).MaximumLength(500);
            field.RuleFor(f => f.DisplayText).MaximumLength(4000);
            field.RuleFor(f => f.SectionKey).MaximumLength(128);

            field.RuleFor(f => f.Options)
                .NotEmpty()
                .When(f => FormFieldTypes.RequiresOptions(f.FieldType))
                .WithMessage("Options are required for Radio Group, Dropdown, and Multi Select field types (BR-075).");

            field.RuleForEach(f => f.Options)
                .ChildRules(opt =>
                {
                    opt.RuleFor(o => o.Value).NotEmpty().MaximumLength(256);
                    opt.RuleFor(o => o.Label).NotEmpty().MaximumLength(500);
                })
                .When(f => f.Options is not null && f.Options.Count > 0);
        });
    }
}
