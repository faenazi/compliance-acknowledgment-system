using Eap.Application.Audience.Abstractions;

namespace Eap.Infrastructure.Audience.Resolution;

/// <summary>
/// Phase 1 stub — returns no members for AD group rules until LDAP group
/// synchronization lands in a later sprint. Audience authoring still captures
/// group references so the data is ready when the resolver is swapped out.
/// </summary>
internal sealed class StubDirectoryGroupResolver : IDirectoryGroupResolver
{
    public Task<IReadOnlyCollection<Guid>> ResolveGroupMembersAsync(
        string groupReference,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<Guid>>(Array.Empty<Guid>());
    }
}
