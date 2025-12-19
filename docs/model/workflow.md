# Workflow Domain Models

## Domain Entities

### Workflow (Aggregate Root)
- WorkflowId, ContentId, AccountId, WorkflowType
- Status (Started, InProgress, Completed, Cancelled, Rejected)
- InitiatedBy, CompletedBy, Duration
- Domain Events: WorkflowStarted, StageCompleted, Approved, Rejected, Reassigned, Completed, Cancelled

### WorkflowStage (Entity)
- StageId, WorkflowId, StageName, StageOrder
- AssignedTo, CompletedBy, Status

### WorkflowApproval (Entity)
- ApprovalId, WorkflowId, ApprovedBy, ApprovalLevel, Comments

## MediatR Commands
- StartWorkflowCommand, CompleteStageCommand
- ApproveWorkflowCommand, RejectWorkflowCommand
- ReassignWorkflowCommand, CancelWorkflowCommand

## MediatR Queries
- GetWorkflowByIdQuery, GetWorkflowsByContentQuery
- GetPendingApprovalsQuery
