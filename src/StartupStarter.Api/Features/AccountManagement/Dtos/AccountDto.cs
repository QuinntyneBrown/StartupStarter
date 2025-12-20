using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Api.Features.AccountManagement.Dtos;

public class AccountDto
{
    public string AccountId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public string OwnerUserId { get; set; } = string.Empty;
    public string SubscriptionTier { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? SuspendedAt { get; set; }
    public string? SuspensionReason { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletionType { get; set; }
}
