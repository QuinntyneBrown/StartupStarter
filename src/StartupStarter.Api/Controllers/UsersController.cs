using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.UserManagement.Commands;
using StartupStarter.Api.Features.UserManagement.Dtos;
using StartupStarter.Api.Features.UserManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id = result.UserId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        var query = new GetUserByIdQuery { UserId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("{id}/activate")]
    public async Task<ActionResult<UserDto>> ActivateUser(string id, [FromBody] ActivateUserCommand command)
    {
        command.UserId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("invitations/{id}")]
    public async Task<ActionResult<UserInvitationDto>> GetInvitation(string id)
    {
        var query = new GetUserInvitationByIdQuery { InvitationId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
