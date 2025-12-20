using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.SystemManagement.Commands;
using StartupStarter.Api.Features.SystemManagement.Dtos;
using StartupStarter.Api.Features.SystemManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("maintenance")]
    public async Task<ActionResult<SystemMaintenanceDto>> ScheduleMaintenance([FromBody] ScheduleMaintenanceCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMaintenance), new { id = result.MaintenanceId }, result);
    }

    [HttpGet("maintenance/{id}")]
    public async Task<ActionResult<SystemMaintenanceDto>> GetMaintenance(string id)
    {
        var query = new GetMaintenanceByIdQuery { MaintenanceId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("maintenance/{id}/start")]
    public async Task<ActionResult<SystemMaintenanceDto>> StartMaintenance(string id)
    {
        var command = new StartMaintenanceCommand { MaintenanceId = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("backups")]
    public async Task<ActionResult<SystemBackupDto>> StartBackup([FromBody] StartBackupCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBackup), new { id = result.BackupId }, result);
    }

    [HttpGet("backups/{id}")]
    public async Task<ActionResult<SystemBackupDto>> GetBackup(string id)
    {
        var query = new GetBackupByIdQuery { BackupId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("errors")]
    public async Task<ActionResult<List<SystemErrorDto>>> GetSystemErrors()
    {
        var query = new GetSystemErrorsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
