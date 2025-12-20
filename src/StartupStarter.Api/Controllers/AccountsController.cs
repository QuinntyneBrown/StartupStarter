using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Authentication.Authorization;
using StartupStarter.Api.Features.AccountManagement.Commands;
using StartupStarter.Api.Features.AccountManagement.Dtos;
using StartupStarter.Api.Features.AccountManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.SuperAdmin},{Roles.AccountAdmin}")]
    public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAccount), new { id = result.AccountId }, result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<AccountDto>> GetAccount(string id)
    {
        var query = new GetAccountByIdQuery { AccountId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
