using Eap.Application.Identity.Abstractions;
using Microsoft.Extensions.Logging;

namespace Eap.Infrastructure.Identity;

/// <summary>
/// Sprint 1 audit sink. Writes structured Serilog events tagged with
/// <c>AuditEvent</c>; a dedicated immutable AuditLog store is introduced
/// with the audit module in a later sprint (BR-130, BR-132).
/// </summary>
internal sealed class IdentityAuditLogger : IIdentityAuditLogger
{
    private readonly ILogger<IdentityAuditLogger> _logger;

    public IdentityAuditLogger(ILogger<IdentityAuditLogger> logger)
    {
        _logger = logger;
    }

    public void LoginSucceeded(Guid userId, string username, string? clientIp) =>
        _logger.LogInformation(
            "{AuditEvent} {UserId} {Username} {ClientIp}",
            "LoginSucceeded", userId, username, clientIp ?? "-");

    public void LoginFailed(string attemptedUsername, string reason, string? clientIp) =>
        _logger.LogWarning(
            "{AuditEvent} {Username} {Reason} {ClientIp}",
            "LoginFailed", attemptedUsername, reason, clientIp ?? "-");

    public void UserProvisioned(Guid userId, string username) =>
        _logger.LogInformation(
            "{AuditEvent} {UserId} {Username}",
            "UserProvisioned", userId, username);

    public void UserSynchronized(Guid userId, string username) =>
        _logger.LogInformation(
            "{AuditEvent} {UserId} {Username}",
            "UserSynchronized", userId, username);

    public void LoggedOut(Guid? userId, string? username) =>
        _logger.LogInformation(
            "{AuditEvent} {UserId} {Username}",
            "LoggedOut", userId, username ?? "-");
}
