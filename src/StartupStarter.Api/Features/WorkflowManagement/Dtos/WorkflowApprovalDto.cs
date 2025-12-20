namespace StartupStarter.Api.Features.WorkflowManagement.Dtos;

public class WorkflowApprovalDto
{
    public string ApprovalId { get; set; } = string.Empty;
    public string StageId { get; set; } = string.Empty;
    public string ApprovedBy { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public DateTime DecidedAt { get; set; }
}
