using Eap.Application.UserPortal.Models;
using Eap.Domain.Forms;
using Eap.Domain.Requirements;

namespace Eap.Application.UserPortal.Abstractions;

/// <summary>
/// Read-optimized repository for user portal queries (Sprint 6).
/// Joins requirements with acknowledgments, policies, and submissions
/// to produce user-centric projections efficiently.
/// </summary>
public interface IUserPortalRepository
{
    /// <summary>Returns dashboard counts and recent items for the given user.</summary>
    Task<MyDashboardDto> GetDashboardAsync(Guid userId, int pendingLimit, int recentLimit, CancellationToken ct);

    /// <summary>Returns a paginated list of the user's current action requirements.</summary>
    Task<(IReadOnlyList<MyActionSummaryDto> Items, int TotalCount)> ListActionsAsync(
        Guid userId, MyActionsFilter filter, CancellationToken ct);

    /// <summary>Returns full detail for one requirement owned by the user.</summary>
    Task<MyActionDetailDto?> GetActionDetailAsync(Guid userId, Guid requirementId, CancellationToken ct);

    /// <summary>Returns a paginated list of the user's past submissions.</summary>
    Task<(IReadOnlyList<MyHistoryItemDto> Items, int TotalCount)> ListHistoryAsync(
        Guid userId, int page, int pageSize, CancellationToken ct);

    /// <summary>Returns full detail for one submission owned by the user.</summary>
    Task<MySubmissionDetailDto?> GetSubmissionDetailAsync(Guid userId, Guid submissionId, CancellationToken ct);

    /// <summary>Finds a pending/overdue requirement that belongs to the user.</summary>
    Task<UserActionRequirement?> FindOpenRequirementAsync(Guid userId, Guid requirementId, CancellationToken ct);

    /// <summary>Checks whether the user already has a submission for the given requirement.</summary>
    Task<bool> HasSubmissionForRequirementAsync(Guid userId, Guid requirementId, CancellationToken ct);

    /// <summary>Finds an existing submission for a specific user + requirement combination.</summary>
    Task<UserSubmission?> FindSubmissionByRequirementAsync(Guid userId, Guid requirementId, CancellationToken ct);

    Task AddSubmissionAsync(UserSubmission submission, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

/// <summary>Filter parameters for the user's actions list.</summary>
public sealed record MyActionsFilter(
    int Page,
    int PageSize,
    UserActionRequirementStatus? Status,
    string? Search);
