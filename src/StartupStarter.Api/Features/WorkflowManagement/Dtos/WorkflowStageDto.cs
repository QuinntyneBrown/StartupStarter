namespace StartupStarter.Api.Features.WorkflowManagement.Dtos;

public class WorkflowStageDto
{
    public string StageId { get; set; } = string.Empty;
    public string WorkflowId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
