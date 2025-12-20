using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.ContentAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.HasKey(e => e.ContentId);

        builder.Property(e => e.ContentId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.Body)
            .IsRequired();

        builder.Property(e => e.AuthorId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.ProfileId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CurrentVersion)
            .IsRequired();

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Configure relationships
        builder.HasMany(e => e.Versions)
            .WithOne(v => v.Content)
            .HasForeignKey(v => v.ContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
