using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.AuditManagement.Commands;
using StartupStarter.Api.Features.AuditManagement.Dtos;
using StartupStarter.Api.Features.AuditManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("logs")]
    public async Task<ActionResult<AuditLogDto>> CreateAuditLog([FromBody] CreateAuditLogCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAuditLog), new { id = result.LogId }, result);
    }

    [HttpGet("logs/{id}")]
    public async Task<ActionResult<AuditLogDto>> GetAuditLog(string id)
    {
        var query = new GetAuditLogByIdQuery { AuditId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("exports")]
    public async Task<ActionResult<AuditExportDto>> RequestExport([FromBody] RequestExportCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetExport), new { id = result.ExportId }, result);
    }

    [HttpGet("exports/{id}")]
    public async Task<ActionResult<AuditExportDto>> GetExport(string id)
    {
        var query = new GetAuditExportByIdQuery { ExportId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
