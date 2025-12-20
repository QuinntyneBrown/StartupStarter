using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AccountAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(e => e.AccountId);

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.AccountType)
            .IsRequired();

        builder.Property(e => e.OwnerUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.SubscriptionTier)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.SuspensionReason)
            .HasMaxLength(500);

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Configure relationships
        builder.HasMany(e => e.Settings)
            .WithOne(s => s.Account)
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
