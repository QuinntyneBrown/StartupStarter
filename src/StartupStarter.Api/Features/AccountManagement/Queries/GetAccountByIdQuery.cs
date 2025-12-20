using MediatR;
using StartupStarter.Api.Features.AccountManagement.Dtos;

namespace StartupStarter.Api.Features.AccountManagement.Queries;

public class GetAccountByIdQuery : IRequest<AccountDto?>
{
    public string AccountId { get; set; } = string.Empty;
}
