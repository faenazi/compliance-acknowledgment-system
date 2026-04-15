using FluentValidation;

namespace Eap.Application.Acknowledgments.Queries.ListAcknowledgmentDefinitions;

public sealed class ListAcknowledgmentDefinitionsQueryValidator
    : AbstractValidator<ListAcknowledgmentDefinitionsQuery>
{
    public ListAcknowledgmentDefinitionsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Search).MaximumLength(256);
        RuleFor(x => x.OwnerDepartment).MaximumLength(256);
    }
}
