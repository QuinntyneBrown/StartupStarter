using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AuditAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.AuditId);

        builder.Property(e => e.AuditId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.EntityType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.EntityId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.PerformedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.IPAddress)
            .HasMaxLength(50);

        builder.Property(e => e.BeforeStateJson)
            .HasMaxLength(4000);

        builder.Property(e => e.AfterStateJson)
            .HasMaxLength(4000);

        builder.Property(e => e.Timestamp)
            .IsRequired();

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);
    }
}
