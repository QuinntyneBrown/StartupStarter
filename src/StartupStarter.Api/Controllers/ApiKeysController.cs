using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.ApiManagement.Commands;
using StartupStarter.Api.Features.ApiManagement.Dtos;
using StartupStarter.Api.Features.ApiManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiKeysController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApiKeysController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiKeyDto>> CreateApiKey([FromBody] CreateApiKeyCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetApiKey), new { id = result.ApiKeyId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiKeyDto>> GetApiKey(string id)
    {
        var query = new GetApiKeyByIdQuery { ApiKeyId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("{id}/revoke")]
    public async Task<ActionResult<ApiKeyDto>> RevokeApiKey(string id, [FromBody] RevokeApiKeyCommand command)
    {
        command.ApiKeyId = id;
        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id}/requests")]
    public async Task<ActionResult<List<ApiRequestDto>>> GetRequests(string id)
    {
        var query = new GetApiRequestsByApiKeyIdQuery { ApiKeyId = id };
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
