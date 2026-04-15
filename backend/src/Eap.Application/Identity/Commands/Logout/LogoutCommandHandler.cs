using Eap.Application.Identity.Abstractions;
using MediatR;

namespace Eap.Application.Identity.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IAuthenticationSession _session;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityAuditLogger _audit;

    public LogoutCommandHandler(
        IAuthenticationSession session,
        ICurrentUser currentUser,
        IIdentityAuditLogger audit)
    {
        _session = session;
        _currentUser = currentUser;
        _audit = audit;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _audit.LoggedOut(_currentUser.UserId, _currentUser.Username);
        await _session.SignOutAsync(cancellationToken).ConfigureAwait(false);
        return Unit.Value;
    }
}
