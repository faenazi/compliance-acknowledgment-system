namespace Eap.Domain.Acknowledgment;

/// <summary>
/// The behaviour expected from a user when they interact with an acknowledgment
/// (docs/03-functional-requirements/functional-requirements.md §FR-022,
/// docs/05-data/conceptual-data-model.md §6.2).
///
/// Sprint 3 introduces the enum and stores it on every definition/version; the
/// downstream behavioural surface (form rendering, commitment workflow, disclosure
/// schema) is delivered in later sprints.
/// </summary>
public enum ActionType
{
    /// <summary>"I have read and understood." Pure confirmation, no payload.</summary>
    SimpleAcknowledgment = 0,

    /// <summary>Confirmation plus a free-text commitment the user accepts explicitly.</summary>
    AcknowledgmentWithCommitment = 1,

    /// <summary>Confirmation plus structured responses captured against a form schema.
    /// Sprint 3 only reserves the enum value; the form builder is delivered in Sprint 5.</summary>
    FormBasedDisclosure = 2,
}
