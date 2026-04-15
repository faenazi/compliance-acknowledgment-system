namespace Eap.Domain.Policy;

/// <summary>
/// Lifecycle state of a <see cref="PolicyVersion"/>
/// (docs/03-functional-requirements/lifecycle-models.md §3.2).
/// </summary>
public enum PolicyVersionStatus
{
    Draft = 0,
    Published = 1,
    Superseded = 2,
    Archived = 3,
}
