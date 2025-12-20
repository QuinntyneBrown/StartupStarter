# Workflow Models

## Core Aggregate

### WorkflowAggregate

Located in: `StartupStarter.Core\Model\WorkflowAggregate\`

#### Folder Structure
```
WorkflowAggregate/
├── Entities/
│   ├── Workflow.cs
│   ├── WorkflowStage.cs
│   └── WorkflowApproval.cs
└── Events/
    ├── WorkflowStartedEvent.cs
    ├── WorkflowStageCompletedEvent.cs
    ├── WorkflowApprovedEvent.cs
    ├── WorkflowRejectedEvent.cs
    ├── WorkflowReassignedEvent.cs
    ├── WorkflowCompletedEvent.cs
    └── WorkflowCancelledEvent.cs
```

#### Entities

Located in: `StartupStarter.Core\Model\WorkflowAggregate\Entities\`

**Workflow.cs** (Aggregate Root)
```csharp
public class Workflow
{
    public string WorkflowId { get; private set; }
    public string ContentId { get; private set; }
    public string AccountId { get; private set; }
    public string WorkflowType { get; private set; }
    public string InitiatedBy { get; private set; }
    public string CurrentAssigneeId { get; private set; }
    public string CurrentStage { get; private set; }
    public string FinalStatus { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public bool IsCompleted { get; private set; }

    private readonly List<WorkflowStage> _stages = new();
    public IReadOnlyCollection<WorkflowStage> Stages => _stages.AsReadOnly();

    private readonly List<WorkflowApproval> _approvals = new();
    public IReadOnlyCollection<WorkflowApproval> Approvals => _approvals.AsReadOnly();

    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void CompleteStage(string stageName, string completedBy);
    public void Approve(string approvedBy, string approvalLevel, string comments);
    public void Reject(string rejectedBy, string rejectionReason, string comments);
    public void Reassign(string previousAssigneeId, string newAssigneeId, string reassignedBy);
    public void Complete(string completedBy, string finalStatus);
    public void Cancel(string cancelledBy, string reason);
}
```

**WorkflowStage.cs**
```csharp
public class WorkflowStage
{
    public string WorkflowStageId { get; private set; }
    public string WorkflowId { get; private set; }
    public string StageName { get; private set; }
    public int StageOrder { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string CompletedBy { get; private set; }
    public bool IsCompleted { get; private set; }

    public Workflow Workflow { get; private set; }
}
```

**WorkflowApproval.cs**
```csharp
public class WorkflowApproval
{
    public string WorkflowApprovalId { get; private set; }
    public string WorkflowId { get; private set; }
    public string ApprovedBy { get; private set; }
    public string ApprovalLevel { get; private set; }
    public string Comments { get; private set; }
    public bool IsApproved { get; private set; }
    public string RejectionReason { get; private set; }
    public DateTime ApprovalDate { get; private set; }

    public Workflow Workflow { get; private set; }
}
```

#### Domain Events

Located in: `StartupStarter.Core\Model\WorkflowAggregate\Events\`

**WorkflowStartedEvent.cs**
```csharp
public class WorkflowStartedEvent : DomainEvent
{
    public string WorkflowId { get; set; }
    public string ContentId { get; set; }
    public string AccountId { get; set; }
    public string WorkflowType { get; set; }
    public string InitiatedBy { get; set; }
    public DateTime Timestamp { get; set; }
}
```

**WorkflowStageCompletedEvent.cs**, **WorkflowApprovedEvent.cs**, **WorkflowRejectedEvent.cs**, **WorkflowReassignedEvent.cs**, **WorkflowCompletedEvent.cs**, **WorkflowCancelledEvent.cs**

## API Layer

### Commands (MediatR)

Located in: `StartupStarter.Api\Features\Workflow\Commands\`

**StartWorkflowCommand.cs**
```csharp
public class StartWorkflowCommand : IRequest<WorkflowDto>
{
    public string ContentId { get; set; }
    public string AccountId { get; set; }
    public string WorkflowType { get; set; }
    public string InitiatedBy { get; set; }
}
```

**CompleteWorkflowStageCommand.cs**, **ApproveWorkflowCommand.cs**, **RejectWorkflowCommand.cs**, **ReassignWorkflowCommand.cs**, **CompleteWorkflowCommand.cs**, **CancelWorkflowCommand.cs**
