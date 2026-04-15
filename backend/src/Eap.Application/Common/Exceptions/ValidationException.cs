using FluentValidation.Results;

namespace Eap.Application.Common.Exceptions;

/// <summary>
/// Raised when FluentValidation produces validation errors.
/// Translated into a 400 response by the global exception middleware.
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}
