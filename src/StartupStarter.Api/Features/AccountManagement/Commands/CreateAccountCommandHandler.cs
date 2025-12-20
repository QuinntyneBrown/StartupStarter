using MediatR;
using StartupStarter.Api.Features.AccountManagement.Dtos;
using StartupStarter.Core;
using StartupStarter.Core.Model.AccountAggregate.Entities;

namespace StartupStarter.Api.Features.AccountManagement.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IStartupStarterContext _context;

    public CreateAccountCommandHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var accountId = Guid.NewGuid().ToString();

        var account = new Account(
            accountId,
            request.AccountName,
            request.AccountType,
            request.OwnerUserId,
            request.SubscriptionTier,
            request.CreatedBy
        );

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        return account.ToDto();
    }
}
