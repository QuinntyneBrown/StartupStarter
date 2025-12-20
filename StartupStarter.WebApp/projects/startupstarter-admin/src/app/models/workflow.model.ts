export enum WorkflowStatus {
  Started = 'Started',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Rejected = 'Rejected',
  Cancelled = 'Cancelled'
}

export enum WorkflowType {
  ContentPublishing = 'ContentPublishing',
  AccountSetup = 'AccountSetup',
  UserApproval = 'UserApproval',
  Custom = 'Custom'
}

export enum ApprovalLevel {
  L1 = 'L1',
  L2 = 'L2',
  L3 = 'L3'
}

export interface Workflow {
  workflowId: string;
  contentId?: string;
  accountId: string;
  workflowType: WorkflowType;
  initiatedBy: string;
  initiatedByName?: string;
  currentAssigneeId?: string;
  currentAssigneeName?: string;
  currentStage: string;
  finalStatus?: WorkflowStatus;
  startedAt: Date;
  completedAt?: Date;
  cancelledAt?: Date;
  duration?: number;
  isCompleted: boolean;
}

export interface WorkflowStage {
  workflowStageId: string;
  workflowId: string;
  stageName: string;
  stageOrder: number;
  completedAt?: Date;
  completedBy?: string;
  completedByName?: string;
  isCompleted: boolean;
}

export interface WorkflowApproval {
  workflowApprovalId: string;
  workflowId: string;
  approvedBy: string;
  approvedByName?: string;
  approvalLevel: ApprovalLevel;
  comments?: string;
  isApproved: boolean;
  rejectionReason?: string;
  approvalDate: Date;
}

export interface StartWorkflowRequest {
  contentId?: string;
  workflowType: WorkflowType;
}

export interface ApproveWorkflowRequest {
  comments?: string;
}

export interface RejectWorkflowRequest {
  reason: string;
  comments?: string;
}

export interface ReassignWorkflowRequest {
  newAssigneeId: string;
  reason: string;
}
