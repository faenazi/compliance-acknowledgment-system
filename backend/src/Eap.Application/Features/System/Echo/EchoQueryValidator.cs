using FluentValidation;

namespace Eap.Application.Features.System.Echo;

public sealed class EchoQueryValidator : AbstractValidator<EchoQuery>
{
    public EchoQueryValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(200);
    }
}
