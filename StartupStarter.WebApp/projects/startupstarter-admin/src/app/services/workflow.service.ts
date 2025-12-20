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

  getById(id: string): Observable<Workflow> {
    return this.api.get<Workflow>(`${this.endpoint}/${id}`);
  }

  getMyTasks(): Observable<Workflow[]> {
    return this.api.get<Workflow[]>(`${this.endpoint}/my-tasks`);
  }

  start(request: StartWorkflowRequest): Observable<Workflow> {
    return this.api.post<Workflow>(this.endpoint, request);
  }

  approve(id: string, request: ApproveWorkflowRequest): Observable<Workflow> {
    return this.api.post<Workflow>(`${this.endpoint}/${id}/approve`, request);
  }

  reject(id: string, request: RejectWorkflowRequest): Observable<Workflow> {
    return this.api.post<Workflow>(`${this.endpoint}/${id}/reject`, request);
  }

  reassign(id: string, request: ReassignWorkflowRequest): Observable<Workflow> {
    return this.api.post<Workflow>(`${this.endpoint}/${id}/reassign`, request);
  }

  cancel(id: string): Observable<Workflow> {
    return this.api.post<Workflow>(`${this.endpoint}/${id}/cancel`, {});
  }

  getStages(id: string): Observable<WorkflowStage[]> {
    return this.api.get<WorkflowStage[]>(`${this.endpoint}/${id}/stages`);
  }

  getHistory(id: string): Observable<WorkflowApproval[]> {
    return this.api.get<WorkflowApproval[]>(`${this.endpoint}/${id}/history`);
  }

  getTypes(): Observable<string[]> {
    return this.api.get<string[]>('workflow-types');
  }
}
