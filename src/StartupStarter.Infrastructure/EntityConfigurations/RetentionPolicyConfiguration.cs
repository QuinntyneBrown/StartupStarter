using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AuditAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class RetentionPolicyConfiguration : IEntityTypeConfiguration<RetentionPolicy>
{
    public void Configure(EntityTypeBuilder<RetentionPolicy> builder)
    {
        builder.HasKey(e => e.RetentionPolicyId);

        builder.Property(e => e.RetentionPolicyId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.PolicyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.RetentionDays)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);
    }
}
