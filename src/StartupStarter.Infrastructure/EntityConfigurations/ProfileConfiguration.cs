using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(e => e.ProfileId);

        builder.Property(e => e.ProfileId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ProfileName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ProfileType)
            .IsRequired();

        builder.Property(e => e.IsDefault)
            .IsRequired();

        builder.Property(e => e.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Ignore DashboardIds collection (this is a private backing field for domain logic)
        builder.Ignore(e => e.DashboardIds);

        // Configure relationships
        builder.HasMany(e => e.Preferences)
            .WithOne(p => p.Profile)
            .HasForeignKey(p => p.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Shares)
            .WithOne(s => s.Profile)
            .HasForeignKey(s => s.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
