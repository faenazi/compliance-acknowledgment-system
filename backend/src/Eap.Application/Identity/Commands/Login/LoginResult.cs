namespace Eap.Application.Identity.Commands.Login;

/// <summary>
/// Result payload returned to the API layer after a successful login.
/// </summary>
public sealed record LoginResult(
    Guid UserId,
    string Username,
    string DisplayName,
    string Email,
    string? Department,
    string? JobTitle,
    IReadOnlyList<string> Roles);
