using Eap.Domain.Audience;

namespace Eap.Application.Audience.Abstractions;

/// <summary>
/// Resolves the effective membership of an <see cref="AudienceDefinition"/>
/// against the local user directory (BR-050..BR-055).
///
/// Inclusion rules are unioned, exclusion rules are subtracted; explicit exclusions
/// always override inclusions. Implementations are free to cap preview samples.
/// </summary>
public interface IAudienceResolver
{
    Task<AudienceResolutionResult> ResolveAsync(
        AudienceDefinition definition,
        AudienceResolutionOptions options,
        CancellationToken cancellationToken);
}

public sealed record AudienceResolutionOptions(
    int SampleSize = 25,
    bool IncludeUserIds = true);

public sealed class AudienceResolutionResult
{
    public required IReadOnlyCollection<Guid> MatchedUserIds { get; init; }

    public int InclusionMatchedCount { get; init; }

    public int ExclusionMatchedCount { get; init; }

    public IReadOnlyList<AudienceMatchedUser> SampleUsers { get; init; } = Array.Empty<AudienceMatchedUser>();
}

public sealed record AudienceMatchedUser(
    Guid UserId,
    string Username,
    string DisplayName,
    string? Department);
