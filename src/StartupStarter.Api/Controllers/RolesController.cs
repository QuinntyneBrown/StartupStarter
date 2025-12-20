using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.RoleManagement.Commands;
using StartupStarter.Api.Features.RoleManagement.Dtos;
using StartupStarter.Api.Features.RoleManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetRole), new { id = result.RoleId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleDto>> GetRole(string id)
    {
        var query = new GetRoleByIdQuery { RoleId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(string id, [FromBody] UpdateRoleCommand command)
    {
        command.RoleId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("assign")]
    public async Task<ActionResult<UserRoleDto>> AssignRole([FromBody] AssignRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("users/{userId}/roles")]
    public async Task<ActionResult<List<UserRoleDto>>> GetUserRoles(string userId)
    {
        var query = new GetUserRolesByUserIdQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
