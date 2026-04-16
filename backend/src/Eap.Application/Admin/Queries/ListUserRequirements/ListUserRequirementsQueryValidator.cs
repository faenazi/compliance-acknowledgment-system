using FluentValidation;

namespace Eap.Application.Admin.Queries.ListUserRequirements;

internal sealed class ListUserRequirementsQueryValidator : AbstractValidator<ListUserRequirementsQuery>
{
    public ListUserRequirementsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
