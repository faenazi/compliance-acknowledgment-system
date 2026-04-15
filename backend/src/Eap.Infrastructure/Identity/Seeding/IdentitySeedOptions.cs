namespace Eap.Infrastructure.Identity.Seeding;

/// <summary>
/// Configuration for reference data seeding (roles and global scope).
/// Bound from the <c>Identity:Seed</c> configuration section.
/// </summary>
public sealed class IdentitySeedOptions
{
    public const string SectionName = "Identity:Seed";

    /// <summary>When false, reference data seeding is skipped on start-up.</summary>
    public bool Enabled { get; set; } = true;
}
