using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(e => e.UserRoleId);

        builder.Property(e => e.UserRoleId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.RoleId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AssignedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AssignedAt)
            .IsRequired();

        builder.Property(e => e.RevokedBy)
            .HasMaxLength(450);

        builder.Property(e => e.RevocationReason)
            .HasMaxLength(500);

        builder.Property(e => e.IsActive)
            .IsRequired();

        // Configure relationship
        builder.HasOne(e => e.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
