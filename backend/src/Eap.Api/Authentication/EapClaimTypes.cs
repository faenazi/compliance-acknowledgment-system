namespace Eap.Api.Authentication;

/// <summary>
/// Application-specific claim type URIs stored on the authenticated principal.
/// These are internal to the EAP API and isolated from ASP.NET Core's
/// <c>ClaimTypes</c> constants so that authorization semantics remain explicit.
/// </summary>
internal static class EapClaimTypes
{
    public const string UserId = "eap:user_id";
    public const string Username = "eap:username";
    public const string DisplayName = "eap:display_name";
    public const string Email = "eap:email";
    public const string Department = "eap:department";

    /// <summary>Encodes a role-scope pair as "roleName|scopeType|scopeReference".</summary>
    public const string Scope = "eap:scope";

    public const string AuthenticationScheme = "EapCookie";
}
