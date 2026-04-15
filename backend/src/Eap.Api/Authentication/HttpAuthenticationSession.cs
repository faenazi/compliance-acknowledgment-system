using System.Globalization;
using System.Security.Claims;
using Eap.Application.Identity.Abstractions;
using Eap.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Eap.Api.Authentication;

/// <summary>
/// ASP.NET Core cookie-based implementation of <see cref="IAuthenticationSession"/>.
/// Keeps authentication concerns (cookie sign-in/out, claim shape) isolated
/// from the application layer.
/// </summary>
internal sealed class HttpAuthenticationSession : IAuthenticationSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpAuthenticationSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task SignInAsync(
        User user,
        IReadOnlyList<UserRoleAssignmentView> assignments,
        CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HttpContext for sign-in.");

        var claims = new List<Claim>
        {
            new(EapClaimTypes.UserId, user.Id.ToString()),
            new(EapClaimTypes.Username, user.Username),
            new(EapClaimTypes.DisplayName, user.DisplayName),
            new(EapClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
        };

        if (!string.IsNullOrWhiteSpace(user.Department))
        {
            claims.Add(new Claim(EapClaimTypes.Department, user.Department));
        }

        foreach (var assignment in assignments)
        {
            claims.Add(new Claim(ClaimTypes.Role, assignment.RoleName));
            claims.Add(new Claim(
                EapClaimTypes.Scope,
                FormatScope(assignment.RoleName, assignment.ScopeType, assignment.ScopeReference)));
        }

        var identity = new ClaimsIdentity(claims, EapClaimTypes.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        return context.SignInAsync(EapClaimTypes.AuthenticationScheme, principal);
    }

    public Task SignOutAsync(CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No active HttpContext for sign-out.");

        return context.SignOutAsync(EapClaimTypes.AuthenticationScheme);
    }

    private static string FormatScope(string roleName, ScopeType type, string reference) =>
        string.Join('|',
            roleName,
            ((int)type).ToString(CultureInfo.InvariantCulture),
            reference ?? string.Empty);
}
