using MediatR;

namespace Eap.Application.Identity.Commands.Login;

/// <summary>
/// Authenticates the supplied credentials against LDAP / AD, provisions or
/// refreshes the local user profile, and establishes an authenticated session.
/// </summary>
public sealed record LoginCommand(string Username, string Password, string? ClientIpAddress)
    : IRequest<LoginResult>;
