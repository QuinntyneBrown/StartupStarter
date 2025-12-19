# Workflow Events

## Workflow.Started
**Description**: Fired when a workflow is initiated for content

**Payload**:
- WorkflowId: string
- ContentId: string
- AccountId: string
- WorkflowType: string
- InitiatedBy: string (UserId)
- Timestamp: DateTime

---

## Workflow.StageCompleted
**Description**: Fired when a workflow stage is completed

**Payload**:
- WorkflowId: string
- ContentId: string
- AccountId: string
- StageName: string
- CompletedBy: string (UserId)
- Timestamp: DateTime

---

## Workflow.Approved
**Description**: Fired when content is approved in workflow

**Payload**:
- WorkflowId: string
- ContentId: string
- AccountId: string
- ApprovedBy: string (UserId)
- ApprovalLevel: string
- Comments: string
- Timestamp: DateTime

---

## Workflow.Rejected
**Description**: Fired when content is rejected in workflow

**Payload**:
- WorkflowId: string
- ContentId: string
- AccountId: string
- RejectedBy: string (UserId)
- RejectionReason: string
- Comments: string
- Timestamp: DateTime

---

## Workflow.Reassigned
**Description**: Fired when workflow task is reassigned to another user

**Payload**:
- WorkflowId: string
- ContentId: string
- AccountId: string
- PreviousAssigneeId: string
- NewAssigneeId: string
- ReassignedBy: string (UserId)
- Timestamp: DateTime

---

## Workflow.Completed
**Description**: Fired when entire workflow is completed

**Payload**:
- WorkflowId: string
- ContentId: string
- AccountId: string
- CompletedBy: string (UserId)
- FinalStatus: string
- Duration: TimeSpan
- Timestamp: DateTime

---

## Workflow.Cancelled
**Description**: Fired when a workflow is cancelled

**Payload**:
- WorkflowId: string
- ContentId: string
- AccountId: string
- CancelledBy: string (UserId)
- Reason: string
- Timestamp: DateTime
