namespace Eap.Application.Common.Exceptions;

/// <summary>
/// Raised when a required domain entity is missing.
/// Translated into a 404 response by the global exception middleware.
/// </summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entity, object key)
        : base($"Entity '{entity}' with key '{key}' was not found.")
    {
    }
}
