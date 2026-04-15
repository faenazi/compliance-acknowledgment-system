using Eap.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eap.Infrastructure.Persistence.Configurations;

internal sealed class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.ToTable("UserRoleAssignments", "identity");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.RoleId).IsRequired();
        builder.Property(a => a.ScopeId).IsRequired();
        builder.Property(a => a.EffectiveFromUtc).IsRequired();
        builder.Property(a => a.IsActive).IsRequired();

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(a => a.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Scope>()
            .WithMany()
            .HasForeignKey(a => a.ScopeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.UserId, a.RoleId, a.ScopeId })
            .HasDatabaseName("IX_UserRoleAssignments_User_Role_Scope");
    }
}
