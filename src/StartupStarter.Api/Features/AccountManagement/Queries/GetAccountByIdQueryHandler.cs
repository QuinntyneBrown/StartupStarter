using MediatR;
using Microsoft.EntityFrameworkCore;
using StartupStarter.Api.Features.AccountManagement.Dtos;
using StartupStarter.Core;

namespace StartupStarter.Api.Features.AccountManagement.Queries;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    private readonly IStartupStarterContext _context;

    public GetAccountByIdQueryHandler(IStartupStarterContext context)
    {
        _context = context;
    }

    public async Task<AccountDto?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountId == request.AccountId, cancellationToken);

        return account?.ToDto();
    }
}
