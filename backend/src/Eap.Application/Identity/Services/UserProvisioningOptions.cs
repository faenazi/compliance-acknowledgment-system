namespace Eap.Application.Identity.Services;

/// <summary>
/// Options that govern how new users are provisioned on first successful login.
/// Bound from the <c>Identity:Provisioning</c> configuration section.
/// </summary>
public sealed class UserProvisioningOptions
{
    public const string SectionName = "Identity:Provisioning";

    /// <summary>
    /// When true, newly-provisioned users automatically receive the built-in
    /// End User role with Global scope. Mirrors BR-143 (every user is at
    /// least an End User) and is configurable for controlled environments.
    /// </summary>
    public bool AssignEndUserRoleOnProvision { get; set; } = true;

    /// <summary>
    /// Usernames (sAMAccountName) that should receive the System Administrator
    /// role with Global scope once provisioned. Intended for bootstrapping
    /// the first administrators without requiring manual DB edits.
    /// </summary>
    public IList<string> SystemAdministrators { get; set; } = new List<string>();
}
