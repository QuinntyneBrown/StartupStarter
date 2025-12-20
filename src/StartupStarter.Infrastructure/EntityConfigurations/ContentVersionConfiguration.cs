using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ContentAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ContentVersionConfiguration : IEntityTypeConfiguration<ContentVersion>
{
    public void Configure(EntityTypeBuilder<ContentVersion> builder)
    {
        builder.HasKey(e => e.ContentVersionId);

        builder.Property(e => e.ContentVersionId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ContentId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.VersionNumber)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.Body)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ChangeDescription)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Configure relationship
        builder.HasOne(e => e.Content)
            .WithMany(c => c.Versions)
            .HasForeignKey(e => e.ContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
