namespace Eap.Application.Identity.Models;

/// <summary>
/// Directory-layer projection of a user, produced by <see cref="Abstractions.ILdapAuthenticator"/>
/// and <see cref="Abstractions.IUserDirectory"/>. Used as the source payload
/// for local profile synchronization. AD remains source-of-truth (BR-061).
/// </summary>
public sealed record LdapUser(
    string Username,
    string DisplayName,
    string Email,
    string? Department,
    string? JobTitle,
    string? DirectoryReference,
    IReadOnlyCollection<string> Groups);
