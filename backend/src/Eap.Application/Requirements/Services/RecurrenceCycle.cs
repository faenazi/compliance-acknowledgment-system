using Eap.Domain.Acknowledgment;

namespace Eap.Application.Requirements.Services;

/// <summary>
/// Derivation helpers for the deterministic cycle-key pattern used by
/// <c>UserActionRequirement.CycleReference</c> (BR-040..BR-047).
///
/// Keys:
/// <list type="bullet">
///   <item><c>onboarding</c> — OnboardingOnly / OnboardingAndAnnual (first cycle).</item>
///   <item><c>annual:YYYY</c> — Annual / OnboardingAndAnnual (subsequent cycles).</item>
///   <item><c>event:&lt;reference&gt;</c> — EventDriven.</item>
///   <item><c>change:&lt;reference&gt;</c> — OnChange.</item>
/// </list>
/// </summary>
public static class RecurrenceCycle
{
    public const string Onboarding = "onboarding";

    public static string AnnualFor(int year) => $"annual:{year:D4}";

    public static string EventFor(string eventReference) =>
        $"event:{(eventReference ?? string.Empty).Trim()}";

    public static string ChangeFor(string changeReference) =>
        $"change:{(changeReference ?? string.Empty).Trim()}";

    /// <summary>
    /// Picks the default cycle key for a generation pass given the version's
    /// recurrence model and the current calendar year. Returns null when the
    /// caller must supply an explicit reference (event / on-change cadences).
    /// </summary>
    public static string? DefaultCycleKey(RecurrenceModel model, int currentYear)
    {
        return model switch
        {
            RecurrenceModel.OnboardingOnly => Onboarding,
            RecurrenceModel.Annual => AnnualFor(currentYear),
            RecurrenceModel.OnboardingAndAnnual => AnnualFor(currentYear),
            RecurrenceModel.OnChange => null,
            RecurrenceModel.EventDriven => null,
            _ => null,
        };
    }
}
