using MediatR;
using Microsoft.AspNetCore.Mvc;
using StartupStarter.Api.Features.WorkflowManagement.Commands;
using StartupStarter.Api.Features.WorkflowManagement.Dtos;
using StartupStarter.Api.Features.WorkflowManagement.Queries;

namespace StartupStarter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkflowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<WorkflowDto>> StartWorkflow([FromBody] StartWorkflowCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetWorkflow), new { id = result.WorkflowId }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkflowDto>> GetWorkflow(string id)
    {
        var query = new GetWorkflowByIdQuery { WorkflowId = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("stages/{stageId}/complete")]
    public async Task<ActionResult<WorkflowStageDto>> CompleteStage(string stageId, [FromBody] CompleteStageCommand command)
    {
        command.StageId = stageId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("stages/{stageId}/approve")]
    public async Task<ActionResult<WorkflowApprovalDto>> ApproveWorkflow(string stageId, [FromBody] ApproveWorkflowCommand command)
    {
        command.StageId = stageId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}/stages")]
    public async Task<ActionResult<List<WorkflowStageDto>>> GetStages(string id)
    {
        var query = new GetWorkflowStagesByWorkflowIdQuery { WorkflowId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
