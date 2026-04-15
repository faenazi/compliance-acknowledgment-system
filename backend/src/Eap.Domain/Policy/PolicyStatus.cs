namespace Eap.Domain.Policy;

/// <summary>
/// Lifecycle state of the master <see cref="Policy"/> record
/// (docs/03-functional-requirements/lifecycle-models.md §3.1).
/// </summary>
public enum PolicyStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2,
}
