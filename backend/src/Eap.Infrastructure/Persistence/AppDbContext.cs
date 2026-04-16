using Eap.Application.Abstractions;
using Eap.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eap.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Username).HasMaxLength(100).IsRequired();
            entity.Property(user => user.FullName).HasMaxLength(200).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
            entity.Property(user => user.Department).HasMaxLength(200);
            entity.Property(user => user.JobTitle).HasMaxLength(200);
            entity.HasIndex(user => user.Username).IsUnique();
        });
    }
}
