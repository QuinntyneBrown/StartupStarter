using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(e => e.SessionId);

        builder.Property(e => e.SessionId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.IPAddress)
            .HasMaxLength(50);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.LoginMethod)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired();

        // Ignore DomainEvents collection
        builder.Ignore(e => e.DomainEvents);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.AccountId);
        builder.HasIndex(e => e.IsActive);
    }
}
