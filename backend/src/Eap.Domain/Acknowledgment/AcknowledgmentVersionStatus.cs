namespace Eap.Domain.Acknowledgment;

/// <summary>
/// Lifecycle state of an <see cref="AcknowledgmentVersion"/>
/// (docs/03-functional-requirements/lifecycle-models.md §4.2). Mirrors the
/// policy-version lifecycle so authoring flows look identical to policy managers.
/// </summary>
public enum AcknowledgmentVersionStatus
{
    Draft = 0,
    Published = 1,
    Superseded = 2,
    Archived = 3,
}
