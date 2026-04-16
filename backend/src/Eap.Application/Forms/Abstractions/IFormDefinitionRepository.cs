using Eap.Domain.Forms;

namespace Eap.Application.Forms.Abstractions;

/// <summary>
/// Persistence abstraction for <see cref="FormDefinition"/> and related queries.
/// </summary>
public interface IFormDefinitionRepository
{
    Task<FormDefinition?> FindByVersionIdAsync(Guid acknowledgmentVersionId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
