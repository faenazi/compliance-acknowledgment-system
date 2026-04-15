using Eap.Application.Audience.Abstractions;
using Eap.Domain.Audience;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Audience.Resolution;

/// <summary>
/// Default audience resolver backed by the local user cache plus
/// <see cref="IDirectoryGroupResolver"/> for AD group expansion (BR-050..BR-055).
///
/// Evaluation pipeline:
/// <list type="number">
///   <item>Build the inclusion set by unioning matches from every inclusion rule.</item>
///   <item>Build the exclusion set by unioning matches from every exclusion rule.</item>
///   <item>Subtract exclusions — explicit exclusions always win (BR-055).</item>
/// </list>
/// </summary>
internal sealed class AudienceResolver : IAudienceResolver
{
    private readonly EapDbContext _db;
    private readonly IDirectoryGroupResolver _groups;

    public AudienceResolver(EapDbContext db, IDirectoryGroupResolver groups)
    {
        _db = db;
        _groups = groups;
    }

    public async Task<AudienceResolutionResult> ResolveAsync(
        AudienceDefinition definition,
        AudienceResolutionOptions options,
        CancellationToken cancellationToken)
    {
        var inclusions = new HashSet<Guid>();
        foreach (var rule in definition.InclusionRules)
        {
            var ids = await ResolveRuleAsync(rule, cancellationToken).ConfigureAwait(false);
            inclusions.UnionWith(ids);
        }

        var exclusions = new HashSet<Guid>();
        foreach (var rule in definition.ExclusionRules)
        {
            var ids = await ResolveRuleAsync(rule, cancellationToken).ConfigureAwait(false);
            exclusions.UnionWith(ids);
        }

        var effective = new HashSet<Guid>(inclusions);
        effective.ExceptWith(exclusions);

        IReadOnlyList<AudienceMatchedUser> sample = Array.Empty<AudienceMatchedUser>();
        if (options.SampleSize > 0 && effective.Count > 0)
        {
            var ids = effective.Take(options.SampleSize).ToList();
            var rows = await _db.Users
                .Where(u => ids.Contains(u.Id))
                .Select(u => new AudienceMatchedUser(u.Id, u.Username, u.DisplayName, u.Department))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            sample = rows;
        }

        return new AudienceResolutionResult
        {
            MatchedUserIds = options.IncludeUserIds ? effective : Array.Empty<Guid>(),
            InclusionMatchedCount = inclusions.Count,
            ExclusionMatchedCount = exclusions.Count,
            SampleUsers = sample,
        };
    }

    private async Task<IReadOnlyCollection<Guid>> ResolveRuleAsync(
        AudienceRule rule,
        CancellationToken cancellationToken)
    {
        switch (rule.RuleType)
        {
            case AudienceRuleType.AllUsers:
                return await _db.Users
                    .Where(u => u.IsActive)
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

            case AudienceRuleType.Department:
                if (string.IsNullOrWhiteSpace(rule.RuleValue))
                {
                    return Array.Empty<Guid>();
                }

                return await _db.Users
                    .Where(u => u.IsActive && u.Department == rule.RuleValue)
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

            case AudienceRuleType.AdGroup:
                if (string.IsNullOrWhiteSpace(rule.RuleValue))
                {
                    return Array.Empty<Guid>();
                }

                return await _groups
                    .ResolveGroupMembersAsync(rule.RuleValue, cancellationToken)
                    .ConfigureAwait(false);

            case AudienceRuleType.User:
                if (!Guid.TryParse(rule.RuleValue, out var userId))
                {
                    return Array.Empty<Guid>();
                }

                var exists = await _db.Users
                    .AnyAsync(u => u.Id == userId && u.IsActive, cancellationToken)
                    .ConfigureAwait(false);
                return exists ? new[] { userId } : Array.Empty<Guid>();

            default:
                return Array.Empty<Guid>();
        }
    }
}
