using Eap.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class ScopeConfiguration : IEntityTypeConfiguration<Scope>
{
    public void Configure(EntityTypeBuilder<Scope> builder)
    {
        builder.ToTable("Scopes", "identity");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.Reference)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(s => s.Description).HasMaxLength(512);

        builder.HasIndex(s => new { s.Type, s.Reference }).IsUnique();
    }
}
