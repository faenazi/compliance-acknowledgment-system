using FluentValidation;

namespace Eap.Application.Acknowledgments.Commands.ArchiveAcknowledgmentVersion;

public sealed class ArchiveAcknowledgmentVersionCommandValidator
    : AbstractValidator<ArchiveAcknowledgmentVersionCommand>
{
    public ArchiveAcknowledgmentVersionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
    }
}
