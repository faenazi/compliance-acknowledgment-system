using FluentValidation;

namespace Eap.Application.Forms.Commands.SubmitForm;

public sealed class SubmitFormCommandValidator : AbstractValidator<SubmitFormCommand>
{
    public SubmitFormCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();

        RuleFor(x => x.SubmissionJson)
            .NotEmpty()
            .WithMessage("Submission data is required.");
    }
}
