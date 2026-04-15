namespace Eap.Domain.Identity;

/// <summary>
/// Canonical system role names used as seed data and as the stable identifier
/// for role-based authorization checks. Role rows are still persisted locally
/// (BR-141 / BR-142 — authorization is application-local, not AD-derived).
/// </summary>
public static class SystemRoles
{
    public const string SystemAdministrator = "SystemAdministrator";
    public const string PolicyManager = "PolicyManager";
    public const string AcknowledgmentManager = "AcknowledgmentManager";
    public const string Publisher = "Publisher";
    public const string ComplianceViewer = "ComplianceViewer";
    public const string Auditor = "Auditor";
    public const string EndUser = "EndUser";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SystemAdministrator,
        PolicyManager,
        AcknowledgmentManager,
        Publisher,
        ComplianceViewer,
        Auditor,
        EndUser,
    };
}
