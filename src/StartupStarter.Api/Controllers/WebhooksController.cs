using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.WebhookManagement.Commands;
using StartupStarter.Api.Features.WebhookManagement.Dtos;
using StartupStarter.Api.Features.WebhookManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public WebhooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<WebhookDto>> RegisterWebhook([FromBody] RegisterWebhookCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetWebhook), new { id = result.WebhookId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WebhookDto>> GetWebhook(string id)
    {
        var query = new GetWebhookByIdQuery { WebhookId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<WebhookDto>> UpdateWebhook(string id, [FromBody] UpdateWebhookCommand command)
    {
        command.WebhookId = id;
        var result = await _mediator.Send(command);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWebhook(string id, [FromBody] DeleteWebhookCommand command)
    {
        command.WebhookId = id;
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/deliveries")]
    public async Task<ActionResult<List<WebhookDeliveryDto>>> GetDeliveries(string id)
    {
        var query = new GetWebhookDeliveriesByWebhookIdQuery { WebhookId = id };
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
