using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.AuthenticationAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
{
    public void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.HasKey(e => e.LoginAttemptId);

        builder.Property(e => e.LoginAttemptId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.IPAddress)
            .HasMaxLength(50);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.LoginMethod)
            .IsRequired();

        builder.Property(e => e.Success)
            .IsRequired();

        builder.Property(e => e.AttemptCount)
            .IsRequired();

        builder.Property(e => e.Timestamp)
            .IsRequired();

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Email);
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.Success);
    }
}
