export interface Workflow {
  workflowId: string;
  name: string;
  contentId: string;
  accountId: string;
  workflowType: string;
  entityType: string;
  initiatedBy: string;
  currentAssigneeId: string;
  currentStage: string;
  status: WorkflowStatus;
  finalStatus?: string;
  startedAt: Date;
  completedAt?: Date;
  cancelledAt?: Date;
  duration?: number;
  isCompleted: boolean;
  steps: WorkflowStage[];
  stages: WorkflowStage[];
  approvals: WorkflowApproval[];
}

export enum WorkflowStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Draft = 'Draft',
  Deprecated = 'Deprecated'
}

export interface WorkflowStage {
  workflowStageId: string;
  workflowId: string;
  stageName: string;
  stageOrder: number;
  completedAt?: Date;
  completedBy?: string;
  isCompleted: boolean;
}

export interface WorkflowApproval {
  workflowApprovalId: string;
  workflowId: string;
  approvedBy: string;
  approvalLevel: string;
  comments: string;
  isApproved: boolean;
  rejectionReason?: string;
  approvalDate: Date;
}

export interface StartWorkflowRequest {
  contentId: string;
  accountId: string;
  workflowType: string;
}

export interface ApproveWorkflowRequest {
  approvalLevel: string;
  comments?: string;
}

export interface RejectWorkflowRequest {
  rejectionReason: string;
  comments?: string;
}

export interface ReassignWorkflowRequest {
  newAssigneeId: string;
}
