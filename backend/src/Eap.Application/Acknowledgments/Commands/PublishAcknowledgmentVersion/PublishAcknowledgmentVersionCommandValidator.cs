using FluentValidation;

namespace Eap.Application.Acknowledgments.Commands.PublishAcknowledgmentVersion;

public sealed class PublishAcknowledgmentVersionCommandValidator
    : AbstractValidator<PublishAcknowledgmentVersionCommand>
{
    public PublishAcknowledgmentVersionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
    }
}
