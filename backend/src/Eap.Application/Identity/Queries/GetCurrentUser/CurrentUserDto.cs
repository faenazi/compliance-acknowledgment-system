using Eap.Domain.Identity;

namespace Eap.Application.Identity.Queries.GetCurrentUser;

public sealed record CurrentUserDto(
    Guid UserId,
    string Username,
    string DisplayName,
    string Email,
    string? Department,
    string? JobTitle,
    IReadOnlyList<string> Roles,
    IReadOnlyList<CurrentUserScopeDto> Scopes);

public sealed record CurrentUserScopeDto(ScopeType Type, string Reference, string RoleName);
