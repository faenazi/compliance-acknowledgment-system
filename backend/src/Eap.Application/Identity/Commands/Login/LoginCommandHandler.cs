using Eap.Application.Identity.Abstractions;
using Eap.Application.Identity.Exceptions;
using MediatR;

namespace Eap.Application.Identity.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ILdapAuthenticator _ldap;
    private readonly IUserProvisioner _provisioner;
    private readonly IUserRepository _users;
    private readonly IAuthenticationSession _session;
    private readonly IIdentityAuditLogger _audit;
    private readonly TimeProvider _clock;

    public LoginCommandHandler(
        ILdapAuthenticator ldap,
        IUserProvisioner provisioner,
        IUserRepository users,
        IAuthenticationSession session,
        IIdentityAuditLogger audit,
        TimeProvider clock)
    {
        _ldap = ldap;
        _provisioner = provisioner;
        _users = users;
        _session = session;
        _audit = audit;
        _clock = clock;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var directoryUser = await _ldap
            .AuthenticateAsync(request.Username, request.Password, cancellationToken)
            .ConfigureAwait(false);

        if (directoryUser is null)
        {
            _audit.LoginFailed(request.Username, "Invalid credentials or user not found.", request.ClientIpAddress);
            throw new AuthenticationFailedException("Invalid username or password.");
        }

        var user = await _provisioner
            .ProvisionFromDirectoryAsync(directoryUser, cancellationToken)
            .ConfigureAwait(false);

        if (!user.IsActive)
        {
            _audit.LoginFailed(request.Username, "User is deactivated locally.", request.ClientIpAddress);
            throw new AuthenticationFailedException("Account is not active.");
        }

        user.RecordSuccessfulLogin(_clock.GetUtcNow());
        await _users.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var assignments = await _users
            .GetActiveAssignmentsAsync(user.Id, cancellationToken)
            .ConfigureAwait(false);

        await _session
            .SignInAsync(user, assignments, cancellationToken)
            .ConfigureAwait(false);

        _audit.LoginSucceeded(user.Id, user.Username, request.ClientIpAddress);

        var roles = assignments
            .Select(a => a.RoleName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new LoginResult(
            user.Id,
            user.Username,
            user.DisplayName,
            user.Email,
            user.Department,
            user.JobTitle,
            roles);
    }
}
