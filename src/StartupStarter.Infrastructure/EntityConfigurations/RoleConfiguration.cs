using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.RoleAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(e => e.RoleId);

        builder.Property(e => e.RoleId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.RoleName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Ignore Permissions collection (this is a private backing field for domain logic)
        builder.Ignore(e => e.Permissions);

        // Configure relationships
        builder.HasMany(e => e.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
