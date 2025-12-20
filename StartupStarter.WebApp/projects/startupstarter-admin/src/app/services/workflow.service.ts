import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  Workflow,
  WorkflowStage,
  WorkflowApproval,
  StartWorkflowRequest,
  ApproveWorkflowRequest,
  RejectWorkflowRequest,
  ReassignWorkflowRequest
} from '../models';

@Injectable({
  providedIn: 'root'
})
export class WorkflowService {
  private readonly api = inject(ApiService);
  private readonly endpoint = 'workflows';

  getAll(): Observable<Workflow[]> {
    return this.api.get<Workflow[]>(this.endpoint);
  }

  getById(workflowId: string): Observable<Workflow> {
    return this.api.get<Workflow>(`${this.endpoint}/${workflowId}`);
  }

  getByAccount(accountId: string): Observable<Workflow[]> {
    return this.api.get<Workflow[]>(`${this.endpoint}/account/${accountId}`);
  }

  getByContent(contentId: string): Observable<Workflow[]> {
    return this.api.get<Workflow[]>(`${this.endpoint}/content/${contentId}`);
  }

  getByAssignee(assigneeId: string): Observable<Workflow[]> {
    return this.api.get<Workflow[]>(`${this.endpoint}/assignee/${assigneeId}`);
  }

  getPending(): Observable<Workflow[]> {
    return this.api.get<Workflow[]>(`${this.endpoint}/pending`);
  }

  start(request: StartWorkflowRequest): Observable<Workflow> {
    return this.api.post<Workflow>(this.endpoint, request);
  }

  approve(workflowId: string, request: ApproveWorkflowRequest): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${workflowId}/approve`, request);
  }

  reject(workflowId: string, request: RejectWorkflowRequest): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${workflowId}/reject`, request);
  }

  reassign(workflowId: string, request: ReassignWorkflowRequest): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${workflowId}/reassign`, request);
  }

  complete(workflowId: string, finalStatus: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${workflowId}/complete`, { finalStatus });
  }

  cancel(workflowId: string, reason: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${workflowId}/cancel`, { reason });
  }

  // Stage operations
  getStages(workflowId: string): Observable<WorkflowStage[]> {
    return this.api.get<WorkflowStage[]>(`${this.endpoint}/${workflowId}/stages`);
  }

  completeStage(workflowId: string, stageName: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${workflowId}/stages/${stageName}/complete`, {});
  }

  // Approval operations
  getApprovals(workflowId: string): Observable<WorkflowApproval[]> {
    return this.api.get<WorkflowApproval[]>(`${this.endpoint}/${workflowId}/approvals`);
  }

  getWorkflowTypes(): Observable<string[]> {
    return this.api.get<string[]>(`${this.endpoint}/types`);
  }

  activate(workflowId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${workflowId}/activate`, {});
  }

  deactivate(workflowId: string): Observable<boolean> {
    return this.api.post<boolean>(`${this.endpoint}/${workflowId}/deactivate`, {});
  }

  delete(workflowId: string): Observable<boolean> {
    return this.api.delete<boolean>(`${this.endpoint}/${workflowId}`);
  }
}
