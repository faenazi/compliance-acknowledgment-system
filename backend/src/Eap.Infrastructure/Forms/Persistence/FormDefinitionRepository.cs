using Eap.Application.Forms.Abstractions;
using Eap.Domain.Forms;
using Eap.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Forms.Persistence;

internal sealed class FormDefinitionRepository : IFormDefinitionRepository
{
    private readonly EapDbContext _db;

    public FormDefinitionRepository(EapDbContext db)
    {
        _db = db;
    }

    public Task<FormDefinition?> FindByVersionIdAsync(
        Guid acknowledgmentVersionId,
        CancellationToken cancellationToken) =>
        _db.FormDefinitions
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.AcknowledgmentVersionId == acknowledgmentVersionId, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _db.SaveChangesAsync(cancellationToken);
}
