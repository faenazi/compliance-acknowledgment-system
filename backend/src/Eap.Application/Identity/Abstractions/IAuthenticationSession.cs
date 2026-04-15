using Eap.Domain.Identity;

namespace Eap.Application.Identity.Abstractions;

/// <summary>
/// Abstraction over the HTTP authentication session (cookie-based in MVP).
/// Keeps the application layer free of ASP.NET Core authentication types.
/// </summary>
public interface IAuthenticationSession
{
    /// <summary>
    /// Signs the user into the current request context with the given profile
    /// and effective role/scope assignments.
    /// </summary>
    Task SignInAsync(
        User user,
        IReadOnlyList<UserRoleAssignmentView> assignments,
        CancellationToken cancellationToken);

    Task SignOutAsync(CancellationToken cancellationToken);
}
