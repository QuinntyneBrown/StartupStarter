using MediatR;
using StartupStarter.Api.Features.AccountManagement.Dtos;
using StartupStarter.Core.Model.AccountAggregate.Enums;

namespace StartupStarter.Api.Features.AccountManagement.Commands;

public class CreateAccountCommand : IRequest<AccountDto>
{
    public string AccountName { get; set; } = string.Empty;
    public AccountType AccountType { get; set; }
    public string OwnerUserId { get; set; } = string.Empty;
    public string SubscriptionTier { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}
