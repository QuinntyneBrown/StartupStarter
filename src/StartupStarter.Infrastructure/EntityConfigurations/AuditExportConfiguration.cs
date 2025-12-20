using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AuditAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class AuditExportConfiguration : IEntityTypeConfiguration<AuditExport>
{
    public void Configure(EntityTypeBuilder<AuditExport> builder)
    {
        builder.HasKey(e => e.ExportId);

        builder.Property(e => e.ExportId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.RequestedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.EndDate)
            .IsRequired();

        builder.Property(e => e.FiltersJson)
            .HasMaxLength(2000);

        builder.Property(e => e.FileFormat)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.RecordCount)
            .IsRequired();

        builder.Property(e => e.FileLocation)
            .HasMaxLength(1000);

        builder.Property(e => e.RequestedAt)
            .IsRequired();

        builder.Property(e => e.CompletedAt);

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);
    }
}
