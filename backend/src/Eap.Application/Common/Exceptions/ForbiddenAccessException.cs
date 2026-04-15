namespace Eap.Application.Common.Exceptions;

/// <summary>
/// Raised when the caller is authenticated but lacks the required role/scope.
/// Translated into a 403 response by the global exception middleware.
/// </summary>
public sealed class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Access to the requested resource is forbidden.")
    {
    }

    public ForbiddenAccessException(string message) : base(message)
    {
    }
}
