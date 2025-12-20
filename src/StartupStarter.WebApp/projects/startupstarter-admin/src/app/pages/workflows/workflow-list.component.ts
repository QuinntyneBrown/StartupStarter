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
  templateUrl: './workflow-list.component.html',
  styleUrl: './workflow-list.component.scss'
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
