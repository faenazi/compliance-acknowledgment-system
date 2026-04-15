using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Eap.Api.Authentication;

/// <summary>
/// Configures the EAP cookie-based authentication scheme. The cookie is
/// HTTP-only, same-site Lax by default (configurable), and never emits
/// redirects for API endpoints — unauthenticated API callers receive 401/403
/// shaped by the global exception middleware.
/// </summary>
internal static class CookieAuthenticationSetup
{
    public static IServiceCollection AddEapCookieAuthentication(this IServiceCollection services)
    {
        services
            .AddAuthentication(EapClaimTypes.AuthenticationScheme)
            .AddCookie(EapClaimTypes.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "eap.session";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);

                // API-only behaviour: no server-side redirects for missing/insufficient auth.
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization();
        return services;
    }
}
