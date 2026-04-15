using FluentValidation;

namespace Eap.Application.Policies.Commands.UploadPolicyDocument;

public sealed class UploadPolicyDocumentCommandValidator : AbstractValidator<UploadPolicyDocumentCommand>
{
    public UploadPolicyDocumentCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(512);

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required.")
            .MaximumLength(128);

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("Uploaded file must not be empty.");

        RuleFor(x => x.Content).NotNull();
    }
}
