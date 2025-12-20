using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.MediaManagement.Commands;
using StartupStarter.Api.Features.MediaManagement.Dtos;
using StartupStarter.Api.Features.MediaManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;

    public MediaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<MediaDto>> UploadMedia([FromBody] UploadMediaCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMedia), new { id = result.MediaId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MediaDto>> GetMedia(string id)
    {
        var query = new GetMediaByIdQuery { MediaId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MediaDto>> UpdateMedia(string id, [FromBody] UpdateMediaCommand command)
    {
        command.MediaId = id;
        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMedia(string id, [FromBody] DeleteMediaCommand command)
    {
        command.MediaId = id;
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
