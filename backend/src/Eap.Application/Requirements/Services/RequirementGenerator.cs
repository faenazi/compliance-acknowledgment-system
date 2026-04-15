using Eap.Application.Acknowledgments.Abstractions;
using Eap.Application.Audience.Abstractions;
using Eap.Application.Common.Exceptions;
using Eap.Application.Requirements.Abstractions;
using Eap.Application.Requirements.Models;
using Eap.Domain.Acknowledgment;
using Eap.Domain.Requirements;

namespace Eap.Application.Requirements.Services;

/// <summary>
/// Default <see cref="IRequirementGenerator"/> implementation.
/// Orchestrates: load version → resolve audience → compute cycle key →
/// upsert requirements → flag superseded cycles as not-current.
///
/// Idempotent by design: existing (user, version, cycle) rows are skipped.
/// </summary>
public sealed class RequirementGenerator : IRequirementGenerator
{
    private readonly IAcknowledgmentRepository _acknowledgments;
    private readonly IAudienceResolver _audienceResolver;
    private readonly IRequirementRepository _requirements;
    private readonly IRequirementAuditLogger _audit;
    private readonly TimeProvider _clock;

    public RequirementGenerator(
        IAcknowledgmentRepository acknowledgments,
        IAudienceResolver audienceResolver,
        IRequirementRepository requirements,
        IRequirementAuditLogger audit,
        TimeProvider clock)
    {
        _acknowledgments = acknowledgments;
        _audienceResolver = audienceResolver;
        _requirements = requirements;
        _audit = audit;
        _clock = clock;
    }

    public async Task<RequirementGenerationSummaryDto> GenerateForVersionAsync(
        Guid acknowledgmentVersionId,
        string? cycleReferenceOverride,
        string? actor,
        CancellationToken cancellationToken)
    {
        var definition = await _acknowledgments
            .FindDefinitionByVersionIdAsync(acknowledgmentVersionId, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("AcknowledgmentVersion", acknowledgmentVersionId);
        var version = definition.Versions.Single(v => v.Id == acknowledgmentVersionId);

        if (version.Status != AcknowledgmentVersionStatus.Published)
        {
            throw new InvalidOperationException(
                "Requirements can only be generated for a published acknowledgment version.");
        }

        if (version.Audience is null || !version.Audience.HasAnyInclusionRule)
        {
            throw new InvalidOperationException(
                "Cannot generate requirements: the version has no audience configured.");
        }

        var nowUtc = _clock.GetUtcNow();

        var cycleKey = cycleReferenceOverride?.Trim();
        if (string.IsNullOrWhiteSpace(cycleKey))
        {
            cycleKey = RecurrenceCycle.DefaultCycleKey(version.RecurrenceModel, nowUtc.Year)
                ?? throw new InvalidOperationException(
                    $"Recurrence model {version.RecurrenceModel} requires an explicit cycle reference.");
        }

        var resolution = await _audienceResolver
            .ResolveAsync(
                version.Audience,
                new AudienceResolutionOptions(SampleSize: 0, IncludeUserIds: true),
                cancellationToken)
            .ConfigureAwait(false);

        var existing = await _requirements
            .ListLatestForVersionAndCycleAsync(version.Id, cycleKey, cancellationToken)
            .ConfigureAwait(false);
        var existingUserIds = existing.Select(r => r.UserId).ToHashSet();

        var current = await _requirements
            .ListCurrentForVersionAsync(version.Id, cancellationToken)
            .ConfigureAwait(false);

        // Flag previous-cycle rows for the same user as not-current when a new cycle is starting.
        var cancelledCount = 0;
        foreach (var prior in current.Where(r => r.CycleReference != cycleKey))
        {
            prior.MarkSupersededByNewerCycle(nowUtc);
            cancelledCount++;
        }

        var instanceDate = DeriveInstanceDate(version, cycleKey, nowUtc);
        var toCreate = new List<UserActionRequirement>();

        foreach (var userId in resolution.MatchedUserIds)
        {
            if (existingUserIds.Contains(userId))
            {
                continue;
            }

            toCreate.Add(new UserActionRequirement(
                userId: userId,
                acknowledgmentVersionId: version.Id,
                cycleReference: cycleKey,
                recurrenceInstanceDate: instanceDate,
                dueDate: version.DueDate,
                assignedAtUtc: nowUtc,
                createdBy: actor));
        }

        if (toCreate.Count > 0)
        {
            await _requirements.AddRangeAsync(toCreate, cancellationToken).ConfigureAwait(false);
        }

        await _requirements.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _audit.RequirementsGenerated(
            version.Id, cycleKey,
            createdCount: toCreate.Count,
            skippedCount: existingUserIds.Count,
            cancelledCount: cancelledCount,
            actor);

        return new RequirementGenerationSummaryDto
        {
            AcknowledgmentVersionId = version.Id,
            CycleReference = cycleKey,
            CreatedCount = toCreate.Count,
            SkippedCount = existingUserIds.Count,
            CancelledCount = cancelledCount,
            GeneratedAtUtc = nowUtc,
        };
    }

    private static DateOnly? DeriveInstanceDate(
        AcknowledgmentVersion version,
        string cycleKey,
        DateTimeOffset nowUtc)
    {
        // For annual cycles the instance date is January 1st of the cycle year.
        if (cycleKey.StartsWith("annual:", StringComparison.Ordinal)
            && int.TryParse(cycleKey.AsSpan("annual:".Length), out var year))
        {
            return new DateOnly(year, 1, 1);
        }

        // For onboarding and event/change cycles the start-date window is the best
        // approximation available in Phase 1 (real generators may pass dates in
        // explicitly when ingesting HR or business events).
        return version.StartDate ?? DateOnly.FromDateTime(nowUtc.UtcDateTime);
    }
}
