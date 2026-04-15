namespace Eap.Domain.Acknowledgment;

/// <summary>
/// Recurrence models supported in Phase 1 (BR-040, FR-060).
/// Kept as an explicit enum — no rule-engine abstraction — so the handful of
/// supported cadences remain readable and deterministic.
/// </summary>
public enum RecurrenceModel
{
    /// <summary>Not yet configured. Publishing is blocked until this is set (BR-033).</summary>
    Unspecified = 0,

    /// <summary>Once per employee upon joining (BR-041).</summary>
    OnboardingOnly = 1,

    /// <summary>Annual cycle (BR-042).</summary>
    Annual = 2,

    /// <summary>Onboarding first, then annual renewal (BR-043).</summary>
    OnboardingAndAnnual = 3,

    /// <summary>Re-submitted when the user declares a relevant change (BR-044).</summary>
    OnChange = 4,

    /// <summary>Submitted when an event occurs (BR-045).</summary>
    EventDriven = 5,
}
