using Eap.Domain.Acknowledgment;

namespace Eap.Application.Admin.Models;

/// <summary>Operational dashboard summary for the admin portal (Sprint 7).</summary>
public sealed class AdminDashboardDto
{
    public int ActivePolicies { get; init; }
    public int ActiveAcknowledgments { get; init; }
    public int PendingUserActions { get; init; }
    public int OverdueUserActions { get; init; }
    public int CompletedUserActions { get; init; }
    public int TotalUserActions { get; init; }

    /// <summary>Completed / Total as a percentage (0–100), rounded to one decimal.</summary>
    public decimal CompletionRate { get; init; }

    public IReadOnlyList<RecentlyPublishedItemDto> RecentlyPublishedPolicies { get; init; } = [];
    public IReadOnlyList<RecentlyPublishedItemDto> RecentlyPublishedAcknowledgments { get; init; } = [];
}

/// <summary>Lightweight summary of a recently published policy or acknowledgment.</summary>
public sealed class RecentlyPublishedItemDto
{
    public Guid Id { get; init; }
    public Guid VersionId { get; init; }
    public string Title { get; init; } = default!;
    public string OwnerDepartment { get; init; } = default!;
    public int VersionNumber { get; init; }
    public DateTimeOffset PublishedAtUtc { get; init; }
}
