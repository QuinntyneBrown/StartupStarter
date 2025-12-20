import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDialog } from '@angular/material/dialog';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent, BadgeType } from '../../components/shared';
import { WorkflowService } from '../../services';
import { Workflow, WorkflowStatus } from '../../models';

@Component({
  selector: 'app-workflow-list',
  standalone: true,
  imports: [
    MatCardModule, MatTableModule, MatButtonModule, MatIconModule, MatMenuModule, MatTabsModule,
    PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  template: `
    <app-page-header title="Workflows" subtitle="Manage approval workflows" icon="account_tree"></app-page-header>

    <mat-card>
      <mat-card-content>
        <mat-tab-group (selectedTabChange)="onTabChange($event.index)">
          <mat-tab label="My Tasks ({{ myTasks().length }})"></mat-tab>
          <mat-tab label="All Workflows"></mat-tab>
        </mat-tab-group>

        @if (isLoading()) {
          <app-loading-spinner message="Loading workflows..."></app-loading-spinner>
        } @else if (displayedWorkflows().length === 0) {
          <app-empty-state icon="account_tree" [title]="selectedTab === 0 ? 'No pending tasks' : 'No workflows'" [message]="selectedTab === 0 ? 'You have no workflows awaiting your approval' : 'No workflows have been started'"></app-empty-state>
        } @else {
          <table mat-table [dataSource]="displayedWorkflows()" class="full-width">
            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let workflow">{{ workflow.workflowType }}</td>
            </ng-container>
            <ng-container matColumnDef="stage">
              <th mat-header-cell *matHeaderCellDef>Current Stage</th>
              <td mat-cell *matCellDef="let workflow">{{ workflow.currentStage }}</td>
            </ng-container>
            <ng-container matColumnDef="initiatedBy">
              <th mat-header-cell *matHeaderCellDef>Initiated By</th>
              <td mat-cell *matCellDef="let workflow">{{ workflow.initiatedByName || workflow.initiatedBy }}</td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let workflow">
                <app-status-badge [label]="workflow.finalStatus || 'In Progress'" [type]="getStatusType(workflow)"></app-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let workflow">
                <button mat-icon-button [matMenuTriggerFor]="menu" [disabled]="workflow.isCompleted"><mat-icon>more_vert</mat-icon></button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="approve(workflow)"><mat-icon>check</mat-icon><span>Approve</span></button>
                  <button mat-menu-item (click)="reject(workflow)"><mat-icon>close</mat-icon><span>Reject</span></button>
                  <button mat-menu-item (click)="cancel(workflow)"><mat-icon color="warn">cancel</mat-icon><span>Cancel</span></button>
                </mat-menu>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`.full-width { width: 100%; } mat-tab-group { margin-bottom: 16px; }`]
})
export class WorkflowListComponent implements OnInit {
  private readonly workflowService = inject(WorkflowService);
  private readonly dialog = inject(MatDialog);

  readonly isLoading = signal(true);
  readonly workflows = signal<Workflow[]>([]);
  readonly myTasks = signal<Workflow[]>([]);
  selectedTab = 0;
  displayedColumns = ['type', 'stage', 'initiatedBy', 'status', 'actions'];

  readonly displayedWorkflows = computed(() => this.selectedTab === 0 ? this.myTasks() : this.workflows());

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.workflowService.getAll().subscribe({ next: (w) => this.workflows.set(w) });
    this.workflowService.getMyTasks().subscribe({
      next: (tasks) => { this.myTasks.set(tasks); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  onTabChange(index: number): void { this.selectedTab = index; }

  getStatusType(workflow: Workflow): BadgeType {
    if (workflow.isCompleted) {
      return workflow.finalStatus === WorkflowStatus.Completed ? 'success' : workflow.finalStatus === WorkflowStatus.Rejected ? 'error' : 'neutral';
    }
    return 'info';
  }

  approve(workflow: Workflow): void {
    this.workflowService.approve(workflow.workflowId, {}).subscribe(() => this.load());
  }

  reject(workflow: Workflow): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Reject Workflow', message: 'Are you sure you want to reject this workflow?', confirmText: 'Reject', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) this.workflowService.reject(workflow.workflowId, { reason: 'Rejected by user' }).subscribe(() => this.load());
    });
  }

  cancel(workflow: Workflow): void {
    this.workflowService.cancel(workflow.workflowId).subscribe(() => this.load());
  }
}
