using StartupStarter.Core.Model.UserAggregate.Entities;

namespace StartupStarter.Api.Features.UserManagement.Dtos;

public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            AccountId = user.AccountId,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Status = user.Status.ToString(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            ActivatedAt = user.ActivatedAt,
            DeactivatedAt = user.DeactivatedAt,
            LockedAt = user.LockedAt,
            LockReason = user.LockReason
        };
    }

    public static UserInvitationDto ToDto(this UserInvitation invitation)
    {
        var status = invitation.IsAccepted ? "Accepted" :
                    invitation.IsExpired ? "Expired" : "Pending";

        return new UserInvitationDto
        {
            InvitationId = invitation.InvitationId,
            AccountId = invitation.AccountId,
            Email = invitation.Email,
            RoleId = invitation.RoleIds.FirstOrDefault() ?? string.Empty,
            Status = status,
            CreatedAt = invitation.SentAt,
            SentAt = invitation.SentAt,
            AcceptedAt = invitation.AcceptedAt,
            ExpiredAt = invitation.IsExpired ? invitation.ExpiresAt : null
        };
    }
}
