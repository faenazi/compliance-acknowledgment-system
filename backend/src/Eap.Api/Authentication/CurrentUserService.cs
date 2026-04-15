using System.Globalization;
using System.Security.Claims;
using Eap.Application.Identity.Abstractions;
using Eap.Domain.Identity;
using Microsoft.AspNetCore.Http;

namespace Eap.Api.Authentication;

/// <summary>
/// Reads the authenticated user context from the current <see cref="HttpContext"/>.
/// Roles and scopes are decoded from the claims shape produced by
/// <see cref="HttpAuthenticationSession"/>.
/// </summary>
internal sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId =>
        Guid.TryParse(Principal?.FindFirstValue(EapClaimTypes.UserId), out var id) ? id : null;

    public string? Username => Principal?.FindFirstValue(EapClaimTypes.Username);

    public string? DisplayName => Principal?.FindFirstValue(EapClaimTypes.DisplayName);

    public string? Email => Principal?.FindFirstValue(EapClaimTypes.Email);

    public string? Department => Principal?.FindFirstValue(EapClaimTypes.Department);

    public IReadOnlyCollection<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? Array.Empty<string>();

    public IReadOnlyCollection<CurrentUserScope> Scopes =>
        Principal?.FindAll(EapClaimTypes.Scope)
            .Select(Parse)
            .Where(s => s is not null)
            .Select(s => s!)
            .ToArray()
        ?? Array.Empty<CurrentUserScope>();

    public bool IsInRole(string roleName) =>
        Principal?.IsInRole(roleName) ?? false;

    public bool HasScope(ScopeType scopeType, string? reference = null)
    {
        foreach (var scope in Scopes)
        {
            if (scope.Type != scopeType)
            {
                continue;
            }

            if (reference is null ||
                string.Equals(scope.Reference, reference, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static CurrentUserScope? Parse(Claim claim)
    {
        var parts = claim.Value.Split('|', count: 3);
        if (parts.Length != 3) return null;
        if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var typeValue))
        {
            return null;
        }

        return new CurrentUserScope((ScopeType)typeValue, parts[2], parts[0]);
    }
}
