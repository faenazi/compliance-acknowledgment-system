using Eap.Application.Forms.Abstractions;
using Eap.Domain.Forms;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Forms.Persistence;

internal sealed class UserSubmissionRepository : IUserSubmissionRepository
{
    private readonly EapDbContext _db;

    public UserSubmissionRepository(EapDbContext db)
    {
        _db = db;
    }

    public Task<UserSubmission?> FindByIdAsync(
        Guid submissionId,
        CancellationToken cancellationToken) =>
        _db.UserSubmissions
            .Include(s => s.FieldValues)
            .FirstOrDefaultAsync(s => s.Id == submissionId, cancellationToken);

    public async Task<(IReadOnlyList<UserSubmission> Items, int TotalCount)> ListByVersionAsync(
        Guid acknowledgmentVersionId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _db.UserSubmissions
            .Where(s => s.AcknowledgmentVersionId == acknowledgmentVersionId)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await query
            .OrderByDescending(s => s.SubmittedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task AddAsync(UserSubmission submission, CancellationToken cancellationToken)
    {
        await _db.UserSubmissions.AddAsync(submission, cancellationToken).ConfigureAwait(false);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _db.SaveChangesAsync(cancellationToken);
}
