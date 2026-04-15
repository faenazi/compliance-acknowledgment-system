namespace Eap.Application.Identity.Exceptions;

/// <summary>
/// Raised when LDAP credentials are rejected or the user cannot be resolved.
/// Translated to a 401 response by the global exception middleware.
/// </summary>
public sealed class AuthenticationFailedException : UnauthorizedAccessException
{
    public AuthenticationFailedException(string message) : base(message)
    {
    }
}
