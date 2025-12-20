using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasKey(e => e.ApiKeyId);

        builder.Property(e => e.ApiKeyId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.KeyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.KeyHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.ExpiresAt);

        builder.Property(e => e.RevokedAt);

        builder.Property(e => e.RevokedBy)
            .HasMaxLength(450);

        builder.Property(e => e.RevocationReason)
            .HasMaxLength(500);

        builder.Property(e => e.IsActive)
            .IsRequired();

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Ignore permissions collection (stored as private field)
        builder.Ignore(e => e.Permissions);
    }
}
