using FluentValidation;

namespace Eap.Application.Policies.Queries.ListPolicies;

public sealed class ListPoliciesQueryValidator : AbstractValidator<ListPoliciesQuery>
{
    public ListPoliciesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(256);
        RuleFor(x => x.OwnerDepartment).MaximumLength(256);
        RuleFor(x => x.Category).MaximumLength(128);
    }
}
