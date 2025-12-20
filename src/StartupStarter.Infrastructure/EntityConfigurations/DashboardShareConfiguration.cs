using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.DashboardAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class DashboardShareConfiguration : IEntityTypeConfiguration<DashboardShare>
{
    public void Configure(EntityTypeBuilder<DashboardShare> builder)
    {
        builder.HasKey(e => e.DashboardShareId);

        builder.Property(e => e.DashboardShareId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.DashboardId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.OwnerUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.SharedWithUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.PermissionLevel)
            .IsRequired();

        builder.Property(e => e.SharedAt)
            .IsRequired();
    }
}
