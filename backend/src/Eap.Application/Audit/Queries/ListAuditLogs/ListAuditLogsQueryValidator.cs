using FluentValidation;

namespace Eap.Application.Audit.Queries.ListAuditLogs;

public sealed class ListAuditLogsQueryValidator : AbstractValidator<ListAuditLogsQuery>
{
    public ListAuditLogsQueryValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
