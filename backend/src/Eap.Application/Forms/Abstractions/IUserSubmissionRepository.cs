using Eap.Domain.Forms;

namespace Eap.Application.Forms.Abstractions;

/// <summary>
/// Persistence abstraction for <see cref="UserSubmission"/> entities.
/// </summary>
public interface IUserSubmissionRepository
{
    Task<UserSubmission?> FindByIdAsync(Guid submissionId, CancellationToken cancellationToken);

    Task<(IReadOnlyList<UserSubmission> Items, int TotalCount)> ListByVersionAsync(
        Guid acknowledgmentVersionId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task AddAsync(UserSubmission submission, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
