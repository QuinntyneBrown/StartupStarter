using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.AccountManagement.Commands;
using StartupStarter.Api.Features.AccountManagement.Dtos;
using StartupStarter.Api.Features.AccountManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> CreateAccount([FromBody] CreateAccountCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAccount), new { id = result.AccountId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetAccount(string id)
    {
        var query = new GetAccountByIdQuery { AccountId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
