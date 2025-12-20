using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.SystemErrorAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class SystemErrorConfiguration : IEntityTypeConfiguration<SystemError>
{
    public void Configure(EntityTypeBuilder<SystemError> builder)
    {
        builder.HasKey(e => e.ErrorId);

        builder.Property(e => e.ErrorId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ErrorType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(e => e.StackTrace)
            .HasMaxLength(4000);

        builder.Property(e => e.Severity)
            .IsRequired();

        builder.Property(e => e.AffectedAccounts)
            .IsRequired();

        builder.Property(e => e.OccurredAt)
            .IsRequired();

        // Ignore DomainEvents collection
        builder.Ignore(e => e.DomainEvents);

        builder.HasIndex(e => e.ErrorType);
        builder.HasIndex(e => e.Severity);
        builder.HasIndex(e => e.OccurredAt);
    }
}
