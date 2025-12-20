using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AccountAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class AccountSettingsConfiguration : IEntityTypeConfiguration<AccountSettings>
{
    public void Configure(EntityTypeBuilder<AccountSettings> builder)
    {
        builder.HasKey(e => e.AccountSettingsId);

        builder.Property(e => e.AccountSettingsId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.SettingsJson)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Configure relationship
        builder.HasOne(e => e.Account)
            .WithMany(a => a.Settings)
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
