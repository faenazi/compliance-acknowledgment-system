using FluentValidation;

namespace Eap.Application.Compliance.Queries.ListNonCompliantUsers;

public sealed class ListNonCompliantUsersQueryValidator
    : AbstractValidator<ListNonCompliantUsersQuery>
{
    public ListNonCompliantUsersQueryValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
