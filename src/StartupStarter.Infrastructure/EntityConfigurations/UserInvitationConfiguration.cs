using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StartupStarter.Core.Model.UserAggregate.Entities;

namespace StartupStarter.Infrastructure.EntityConfigurations;

public class UserInvitationConfiguration : IEntityTypeConfiguration<UserInvitation>
{
    public void Configure(EntityTypeBuilder<UserInvitation> builder)
    {
        builder.HasKey(e => e.InvitationId);

        builder.Property(e => e.InvitationId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(e => e.AccountId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.InvitedBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(e => e.SentAt)
            .IsRequired();

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.IsAccepted)
            .IsRequired();

        builder.Property(e => e.AcceptedByUserId)
            .HasMaxLength(450);

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);

        // Ignore RoleIds collection (this is a private backing field for domain logic)
        builder.Ignore(e => e.RoleIds);

        // Ignore computed property
        builder.Ignore(e => e.IsExpired);
    }
}
