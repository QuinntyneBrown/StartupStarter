using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.DashboardManagement.Commands;
using StartupStarter.Api.Features.DashboardManagement.Dtos;
using StartupStarter.Api.Features.DashboardManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<DashboardDto>> CreateDashboard([FromBody] CreateDashboardCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetDashboard), new { id = result.DashboardId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DashboardDto>> GetDashboard(string id)
    {
        var query = new GetDashboardByIdQuery { DashboardId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DashboardDto>> UpdateDashboard(string id, [FromBody] UpdateDashboardCommand command)
    {
        command.DashboardId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/cards")]
    public async Task<ActionResult<DashboardCardDto>> AddCard(string id, [FromBody] AddCardCommand command)
    {
        command.DashboardId = id;
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCards), new { id = id }, result);
    }

    [HttpGet("{id}/cards")]
    public async Task<ActionResult<List<DashboardCardDto>>> GetCards(string id)
    {
        var query = new GetDashboardCardsByDashboardIdQuery { DashboardId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
