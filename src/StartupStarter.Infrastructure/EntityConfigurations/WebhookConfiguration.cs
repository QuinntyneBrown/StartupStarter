using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.WebhookAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class WebhookConfiguration : IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.HasKey(e => e.WebhookId);

        builder.Property(e => e.WebhookId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.Url)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.RegisteredBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.RegisteredAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt);

        builder.Property(e => e.DeletedBy)
            .HasMaxLength(450);

        builder.Property(e => e.IsActive)
            .IsRequired();

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Ignore events collection (stored as private field)
        builder.Ignore(e => e.Events);

        // Configure relationships
        builder.HasMany(e => e.Deliveries)
            .WithOne(d => d.Webhook)
            .HasForeignKey(d => d.WebhookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
