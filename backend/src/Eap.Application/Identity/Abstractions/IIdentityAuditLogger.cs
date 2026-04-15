namespace Eap.Application.Identity.Abstractions;

/// <summary>
/// Narrow audit surface for authentication-related events.
/// In Sprint 1 this writes structured Serilog entries so evidence exists
/// from day one. A dedicated immutable AuditLog persistence store is
/// introduced as a separate module in a later sprint — consumers of this
/// interface do not need to change when that happens.
/// </summary>
public interface IIdentityAuditLogger
{
    void LoginSucceeded(Guid userId, string username, string? clientIp);

    void LoginFailed(string attemptedUsername, string reason, string? clientIp);

    void UserProvisioned(Guid userId, string username);

    void UserSynchronized(Guid userId, string username);

    void LoggedOut(Guid? userId, string? username);
}
