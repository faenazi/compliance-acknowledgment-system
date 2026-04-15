using Eap.Application.Identity.Abstractions;
using MediatR;

namespace Eap.Application.Identity.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto?>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _users;

    public GetCurrentUserQueryHandler(ICurrentUser currentUser, IUserRepository users)
    {
        _currentUser = currentUser;
        _users = users;
    }

    public async Task<CurrentUserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not { } userId)
        {
            return null;
        }

        var user = await _users.FindByIdAsync(userId, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        var scopes = _currentUser.Scopes
            .Select(s => new CurrentUserScopeDto(s.Type, s.Reference, s.RoleName))
            .ToArray();

        var roles = _currentUser.Roles
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new CurrentUserDto(
            user.Id,
            user.Username,
            user.DisplayName,
            user.Email,
            user.Department,
            user.JobTitle,
            roles,
            scopes);
    }
}
