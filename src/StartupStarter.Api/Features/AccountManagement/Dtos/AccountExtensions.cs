using StartupStarter.Core.Model.AccountAggregate.Entities;

namespace StartupStarter.Api.Features.AccountManagement.Dtos;

public static class AccountExtensions
{
    public static AccountDto ToDto(this Account account)
    {
        return new AccountDto
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            AccountType = account.AccountType.ToString(),
            OwnerUserId = account.OwnerUserId,
            SubscriptionTier = account.SubscriptionTier,
            Status = account.Status.ToString(),
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt,
            SuspendedAt = account.SuspendedAt,
            SuspensionReason = account.SuspensionReason,
            DeletedAt = account.DeletedAt,
            DeletionType = null
        };
    }
}
