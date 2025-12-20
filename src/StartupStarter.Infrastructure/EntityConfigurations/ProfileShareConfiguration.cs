using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ProfileShareConfiguration : IEntityTypeConfiguration<ProfileShare>
{
    public void Configure(EntityTypeBuilder<ProfileShare> builder)
    {
        builder.HasKey(e => e.ProfileShareId);

        builder.Property(e => e.ProfileShareId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ProfileId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.OwnerUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.SharedWithUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.PermissionLevel)
            .IsRequired();

        builder.Property(e => e.SharedAt)
            .IsRequired();

        // Configure relationship
        builder.HasOne(e => e.Profile)
            .WithMany(p => p.Shares)
            .HasForeignKey(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
