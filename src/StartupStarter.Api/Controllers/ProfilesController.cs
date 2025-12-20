using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.ProfileManagement.Commands;
using StartupStarter.Api.Features.ProfileManagement.Dtos;
using StartupStarter.Api.Features.ProfileManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ProfileDto>> CreateProfile([FromBody] CreateProfileCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProfile), new { id = result.ProfileId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(string id)
    {
        var query = new GetProfileByIdQuery { ProfileId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProfileDto>> UpdateProfile(string id, [FromBody] UpdateProfileCommand command)
    {
        command.ProfileId = id;
        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProfile(string id, [FromBody] DeleteProfileCommand command)
    {
        command.ProfileId = id;
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/preferences")]
    public async Task<ActionResult<List<ProfilePreferencesDto>>> GetPreferences(string id)
    {
        var query = new GetProfilePreferencesByProfileIdQuery { ProfileId = id };
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
