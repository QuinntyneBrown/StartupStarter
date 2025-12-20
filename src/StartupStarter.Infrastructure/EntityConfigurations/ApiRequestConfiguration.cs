using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ApiKeyAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ApiRequestConfiguration : IEntityTypeConfiguration<ApiRequest>
{
    public void Configure(EntityTypeBuilder<ApiRequest> builder)
    {
        builder.HasKey(e => e.RequestId);

        builder.Property(e => e.RequestId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.Endpoint)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.Method)
            .IsRequired();

        builder.Property(e => e.ApiKeyId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.IPAddress)
            .HasMaxLength(50);

        builder.Property(e => e.Timestamp)
            .IsRequired();

        builder.Property(e => e.ResponseStatusCode)
            .IsRequired();

        builder.Property(e => e.ResponseTimeMs)
            .IsRequired();

        builder.Property(e => e.WasRateLimited)
            .IsRequired();

        // Configure relationship with ApiKey
        builder.HasOne(e => e.ApiKey)
            .WithMany()
            .HasForeignKey(e => e.ApiKeyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
