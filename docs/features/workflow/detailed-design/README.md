# Workflow Management - Detailed Design

## Overview
Approval workflow system for content publishing and other business processes.

## Aggregates
- **WorkflowAggregate**: Workflow instances with stages and approvals

## Key Features
- Multi-stage approval workflows
- Configurable workflow types
- Task assignment and reassignment
- Approval/rejection with comments
- Workflow templates
- SLA tracking
- Workflow cancellation
- Audit trail

## Dependencies
- **ContentAggregate**: Primary use case is content approval
- **UserAggregate**: Workflow participants
- **RoleAggregate**: Role-based assignments
- **Notification Service**: Task notifications

## Business Rules

### Workflow Lifecycle
1. Workflow started when content submitted for review
2. Progresses through defined stages
3. Each stage requires approval
4. Can be reassigned if needed
5. Rejection sends back to author
6. Completion publishes content
7. Can be cancelled anytime by author/admin

### Stage Rules
1. Stages executed sequentially
2. Each stage has assigned approver
3. Stage completion advances workflow
4. Can skip optional stages
5. Required stages must complete

### Approval Rules
1. Approver must have appropriate role
2. Comments optional for approval
3. Comments mandatory for rejection
4. Approval levels: L1, L2, L3 (escalating)
5. Rejection requires reason
6. Rejection resets to first stage

### Reassignment Rules
1. Can reassign to user with same or higher role
2. Reason required for reassignment
3. Original assignee notified
4. New assignee notified
5. Reassignment tracked in audit

## Data Model

**Workflows Table**
- WorkflowId (PK), ContentId
- AccountId, WorkflowType
- InitiatedBy, CurrentAssigneeId
- CurrentStage, FinalStatus
- StartedAt, CompletedAt, CancelledAt
- Duration, IsCompleted

**WorkflowStages Table**
- WorkflowStageId (PK), WorkflowId (FK)
- StageName, StageOrder
- CompletedAt, CompletedBy
- IsCompleted

**WorkflowApprovals Table**
- WorkflowApprovalId (PK), WorkflowId (FK)
- ApprovedBy, ApprovalLevel
- Comments, IsApproved
- RejectionReason, ApprovalDate

## Workflow Types

### Content Publishing Workflow
```
1. Submission → Author submits content
2. Editor Review → Editor reviews
3. Manager Approval → Manager approves
4. Publication → Content published
```

### Account Setup Workflow
```
1. Request → User requests new account
2. Security Review → Security approves
3. Admin Approval → Admin approves
4. Provisioning → Account created
```

### Custom Workflows
- Configurable stages
- Role-based assignments
- Conditional logic
- Parallel approvals (future)

## Sequence: Approve Workflow
```
Approver → ApproveWorkflowCommand
→ Validate user is current assignee
→ Validate workflow in correct stage
→ Create WorkflowApproval record
→ Advance to next stage OR complete
→ Assign to next approver
→ Update workflow status
→ Save to database
→ Publish WorkflowApprovedEvent
→ Notify next approver or author
```

## Sequence: Reject Workflow
```
Approver → RejectWorkflowCommand
→ Validate user is current assignee
→ Validate rejection reason provided
→ Create WorkflowApproval (rejected)
→ Reset to first stage
→ Assign back to author
→ Update workflow status
→ Save to database
→ Publish WorkflowRejectedEvent
→ Notify author with comments
```

## API Endpoints
- POST /api/workflows - Start workflow
- GET /api/workflows/{id} - Get workflow
- GET /api/workflows - List workflows
- GET /api/workflows/my-tasks - Get assigned tasks
- POST /api/workflows/{id}/approve - Approve stage
- POST /api/workflows/{id}/reject - Reject workflow
- POST /api/workflows/{id}/reassign - Reassign workflow
- POST /api/workflows/{id}/cancel - Cancel workflow
- GET /api/workflows/{id}/history - Get approval history
- GET /api/workflow-types - List workflow types

## SLA Tracking

**Default SLAs by Stage:**
- Submission → Editor: 24 hours
- Editor → Manager: 48 hours
- Manager → Publication: 24 hours

**SLA Alerts:**
- 50% elapsed → Warning to assignee
- 75% elapsed → Escalation to manager
- 100% elapsed → Escalation to admin

## State Machine
```
[Started] → Approve All Stages → [Completed]
          → Reject Any Stage → [Rejected] → [Started]
          → Cancel → [Cancelled]

[Rejected] → Resubmit → [Started]
```

## Notifications

**When Workflow Started:**
- Notify first approver

**When Stage Approved:**
- Notify next approver
- Or notify author if completed

**When Workflow Rejected:**
- Notify author with rejection reason

**When Workflow Reassigned:**
- Notify old and new assignees

**SLA Notifications:**
- Warning at 50%
- Escalation at 75%, 100%

## Performance
- Workflow states cached
- My Tasks endpoint optimized
- Index on CurrentAssigneeId
- Pagination for workflow lists
- Async notifications

## Metrics
- Average approval time per stage
- SLA compliance rate
- Rejection rate by stage
- Workflow completion rate
- Average workflow duration

## Extensibility
Future enhancements:
- Parallel approval stages
- Conditional branching
- Dynamic stage generation
- External system integration
- Workflow templates library
