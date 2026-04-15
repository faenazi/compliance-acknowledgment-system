using FluentValidation;

namespace Eap.Application.Acknowledgments.Commands.ArchiveAcknowledgmentDefinition;

public sealed class ArchiveAcknowledgmentDefinitionCommandValidator
    : AbstractValidator<ArchiveAcknowledgmentDefinitionCommand>
{
    public ArchiveAcknowledgmentDefinitionCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
    }
}
