using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.BackupAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class SystemBackupConfiguration : IEntityTypeConfiguration<SystemBackup>
{
    public void Configure(EntityTypeBuilder<SystemBackup> builder)
    {
        builder.HasKey(e => e.BackupId);

        builder.Property(e => e.BackupId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.BackupType)
            .IsRequired();

        builder.Property(e => e.StartedAt)
            .IsRequired();

        builder.Property(e => e.BackupLocation)
            .HasMaxLength(500);

        builder.Property(e => e.Success)
            .IsRequired();

        builder.Property(e => e.FailureReason)
            .HasMaxLength(1000);

        // Ignore DomainEvents collection
        builder.Ignore(e => e.DomainEvents);

        builder.HasIndex(e => e.BackupType);
        builder.HasIndex(e => e.StartedAt);
        builder.HasIndex(e => e.Success);
    }
}
