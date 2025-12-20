using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.MaintenanceAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class SystemMaintenanceConfiguration : IEntityTypeConfiguration<SystemMaintenance>
{
    public void Configure(EntityTypeBuilder<SystemMaintenance> builder)
    {
        builder.HasKey(e => e.MaintenanceId);

        builder.Property(e => e.MaintenanceId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ScheduledStartTime)
            .IsRequired();

        builder.Property(e => e.EstimatedDuration)
            .IsRequired();

        builder.Property(e => e.MaintenanceType)
            .IsRequired();

        // Configure AffectedServices as a collection stored as JSON or separate table
        // For simplicity, storing as JSON string
        builder.Property<string>("_affectedServicesJson")
            .HasColumnName("AffectedServices")
            .HasMaxLength(2000);

        builder.Ignore(e => e.AffectedServices);

        // Ignore DomainEvents collection
        builder.Ignore(e => e.DomainEvents);

        builder.HasIndex(e => e.ScheduledStartTime);
        builder.HasIndex(e => e.MaintenanceType);
    }
}
