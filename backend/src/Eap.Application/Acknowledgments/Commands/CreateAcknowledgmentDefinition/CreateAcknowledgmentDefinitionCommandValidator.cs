using FluentValidation;

namespace Eap.Application.Acknowledgments.Commands.CreateAcknowledgmentDefinition;

public sealed class CreateAcknowledgmentDefinitionCommandValidator
    : AbstractValidator<CreateAcknowledgmentDefinitionCommand>
{
    public CreateAcknowledgmentDefinitionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(256);

        RuleFor(x => x.OwnerDepartment)
            .NotEmpty().WithMessage("Owner department is required.")
            .MaximumLength(256);

        RuleFor(x => x.DefaultActionType)
            .IsInEnum().WithMessage("Action type is invalid.");

        RuleFor(x => x.Description)
            .MaximumLength(4000);
    }
}
