using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.WebhookAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
{
    public void Configure(EntityTypeBuilder<WebhookDelivery> builder)
    {
        builder.HasKey(e => e.WebhookDeliveryId);

        builder.Property(e => e.WebhookDeliveryId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.WebhookId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.EventType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.PayloadJson)
            .HasMaxLength(4000);

        builder.Property(e => e.ResponseStatus)
            .IsRequired();

        builder.Property(e => e.Success)
            .IsRequired();

        builder.Property(e => e.FailureReason)
            .HasMaxLength(1000);

        builder.Property(e => e.RetryCount)
            .IsRequired();

        builder.Property(e => e.Timestamp)
            .IsRequired();
    }
}
