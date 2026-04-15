using FluentValidation;

namespace Eap.Application.Audience.Commands.SetAllUsersAudience;

public sealed class SetAllUsersAudienceCommandValidator
    : AbstractValidator<SetAllUsersAudienceCommand>
{
    public SetAllUsersAudienceCommandValidator()
    {
        RuleFor(x => x.DefinitionId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
    }
}
