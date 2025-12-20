using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.ContentManagement.Commands;
using StartupStarter.Api.Features.ContentManagement.Dtos;
using StartupStarter.Api.Features.ContentManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ContentDto>> CreateContent([FromBody] CreateContentCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetContent), new { id = result.ContentId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContentDto>> GetContent(string id)
    {
        var query = new GetContentByIdQuery { ContentId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ContentDto>> UpdateContent(string id, [FromBody] UpdateContentCommand command)
    {
        command.ContentId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/publish")]
    public async Task<ActionResult<ContentDto>> PublishContent(string id, [FromBody] PublishContentCommand command)
    {
        command.ContentId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}/versions/{versionId}")]
    public async Task<ActionResult<ContentVersionDto>> GetVersion(string id, string versionId)
    {
        var query = new GetContentVersionByIdQuery
        {
            ContentId = id,
            VersionId = versionId
        };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
