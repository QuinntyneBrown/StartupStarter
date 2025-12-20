using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class MultiFactorAuthenticationConfiguration : IEntityTypeConfiguration<MultiFactorAuthentication>
{
    public void Configure(EntityTypeBuilder<MultiFactorAuthentication> builder)
    {
        builder.HasKey(e => e.MfaId);

        builder.Property(e => e.MfaId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Method)
            .IsRequired();

        builder.Property(e => e.IsEnabled)
            .IsRequired();

        builder.Property(e => e.EnabledBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EnabledAt)
            .IsRequired();

        builder.Property(e => e.DisabledBy)
            .HasMaxLength(100);

        builder.Property(e => e.DisabledReason)
            .HasMaxLength(500);

        builder.Property(e => e.SecretKey)
            .HasMaxLength(500);

        builder.Property(e => e.BackupCodesJson)
            .HasMaxLength(2000);

        // Ignore DomainEvents collection
        builder.Ignore(e => e.DomainEvents);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.AccountId);
        builder.HasIndex(e => e.IsEnabled);
    }
}
