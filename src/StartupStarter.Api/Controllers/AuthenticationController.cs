using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.AuthenticationManagement.Commands;
using StartupStarter.Api.Features.AuthenticationManagement.Dtos;
using StartupStarter.Api.Features.AuthenticationManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("sessions")]
    public async Task<ActionResult<UserSessionDto>> CreateSession([FromBody] CreateSessionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSession), new { id = result.SessionId }, result);
    }

    [HttpGet("sessions/{id}")]
    public async Task<ActionResult<UserSessionDto>> GetSession(string id)
    {
        var query = new GetSessionByIdQuery { SessionId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("sessions/{id}/end")]
    public async Task<ActionResult> EndSession(string id, [FromBody] EndSessionCommand command)
    {
        command.SessionId = id;
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("mfa/enable")]
    public async Task<ActionResult<MfaDto>> EnableMfa([FromBody] EnableMfaCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("users/{userId}/login-attempts")]
    public async Task<ActionResult<List<LoginAttemptDto>>> GetLoginAttempts(string userId)
    {
        var query = new GetLoginAttemptsByUserIdQuery { UserId = userId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
