using Eap.Domain.Identity;

namespace Eap.Application.Identity.Abstractions;

/// <summary>
/// Applies bootstrap role assignments to a user on first provisioning.
/// Separated from <see cref="IUserProvisioner"/> so that the policy for
/// "what a newly-joined user receives" can evolve without touching the
/// profile-sync logic.
/// </summary>
public interface IDefaultRoleAssigner
{
    Task ApplyAsync(User user, CancellationToken cancellationToken);
}
