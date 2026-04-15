namespace Eap.Domain.Acknowledgment;

/// <summary>
/// Lifecycle state of an <see cref="AcknowledgmentDefinition"/>
/// (docs/03-functional-requirements/lifecycle-models.md §4.1).
/// </summary>
public enum AcknowledgmentStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
}
