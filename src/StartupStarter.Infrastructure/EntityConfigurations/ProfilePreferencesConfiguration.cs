using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ProfileAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ProfilePreferencesConfiguration : IEntityTypeConfiguration<ProfilePreferences>
{
    public void Configure(EntityTypeBuilder<ProfilePreferences> builder)
    {
        builder.HasKey(e => e.ProfilePreferencesId);

        builder.Property(e => e.ProfilePreferencesId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ProfileId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.PreferencesJson)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Configure relationship
        builder.HasOne(e => e.Profile)
            .WithMany(p => p.Preferences)
            .HasForeignKey(e => e.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
