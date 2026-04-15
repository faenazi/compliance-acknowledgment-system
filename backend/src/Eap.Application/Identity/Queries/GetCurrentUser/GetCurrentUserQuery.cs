using MediatR;

namespace Eap.Application.Identity.Queries.GetCurrentUser;

/// <summary>
/// Resolves the profile of the currently authenticated caller.
/// Returns <see langword="null"/> when no session exists.
/// </summary>
public sealed record GetCurrentUserQuery : IRequest<CurrentUserDto?>;
