using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.MediaAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(e => e.MediaId);

        builder.Property(e => e.MediaId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.FileType)
            .HasMaxLength(100);

        builder.Property(e => e.FileSize)
            .IsRequired();

        builder.Property(e => e.UploadedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ProfileId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.StorageLocation)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.UploadedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.DeletedAt);

        builder.Property(e => e.DeletionType);

        builder.Property(e => e.ProcessingStatus)
            .HasMaxLength(50);

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Configure collections as JSON or separate table (depending on requirements)
        // For simplicity, using Ignore to store them as private fields
        builder.Ignore(e => e.Tags);
        builder.Ignore(e => e.Categories);
        builder.Ignore(e => e.OutputFormats);
    }
}
