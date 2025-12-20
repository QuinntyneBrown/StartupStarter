using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.DashboardAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
    public void Configure(EntityTypeBuilder<Dashboard> builder)
    {
        builder.HasKey(e => e.DashboardId);

        builder.Property(e => e.DashboardId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.DashboardName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ProfileId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.IsDefault)
            .IsRequired();

        builder.Property(e => e.Template)
            .HasMaxLength(500);

        builder.Property(e => e.LayoutType)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.DeletedAt);

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Configure relationships
        builder.HasMany(e => e.Cards)
            .WithOne(c => c.Dashboard)
            .HasForeignKey(c => c.DashboardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Shares)
            .WithOne(s => s.Dashboard)
            .HasForeignKey(s => s.DashboardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
