using Eap.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eap.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<AppUser> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
