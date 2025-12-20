using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.DashboardAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class DashboardCardConfiguration : IEntityTypeConfiguration<DashboardCard>
{
    public void Configure(EntityTypeBuilder<DashboardCard> builder)
    {
        builder.HasKey(e => e.CardId);

        builder.Property(e => e.CardId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.DashboardId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.CardType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.ConfigurationJson)
            .HasMaxLength(4000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Configure CardPosition as owned entity
        builder.OwnsOne(e => e.Position, position =>
        {
            position.Property(p => p.Row).IsRequired();
            position.Property(p => p.Column).IsRequired();
            position.Property(p => p.Width).IsRequired();
            position.Property(p => p.Height).IsRequired();
        });
    }
}
