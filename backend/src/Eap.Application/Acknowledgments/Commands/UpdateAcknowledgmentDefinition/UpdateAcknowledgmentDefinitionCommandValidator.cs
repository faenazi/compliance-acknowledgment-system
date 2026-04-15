using FluentValidation;

namespace Eap.Application.Acknowledgments.Commands.UpdateAcknowledgmentDefinition;

public sealed class UpdateAcknowledgmentDefinitionCommandValidator
    : AbstractValidator<UpdateAcknowledgmentDefinitionCommand>
{
    public UpdateAcknowledgmentDefinitionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();

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
