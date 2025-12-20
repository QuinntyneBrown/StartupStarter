import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { PageHeader } from '../../components/shared/page-header';
import { StatusBadge } from '../../components/shared/status-badge';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { EmptyState } from '../../components/shared/empty-state';
import { ConfirmDialog } from '../../components/shared/confirm-dialog';
import { WorkflowService } from '../../services';
import { Workflow, WorkflowStatus } from '../../models';

@Component({
  selector: 'app-workflow-list',
  standalone: true,
  imports: [
    RouterLink,
    FormsModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    PageHeader,
    StatusBadge,
    LoadingSpinner,
    EmptyState
  ],
  template: `
    <app-page-header
      title="Workflows"
      subtitle="Manage workflow definitions"
      icon="account_tree"
    >
      <button mat-raised-button color="primary" routerLink="/workflows/new">
        <mat-icon>add</mat-icon>
        New Workflow
      </button>
    </app-page-header>

    <mat-card class="workflow-list__card">
      <mat-card-content>
        <div class="workflow-list__toolbar">
          <mat-form-field appearance="outline" class="workflow-list__search">
            <mat-label>Search workflows</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by name...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading workflows..."></app-loading-spinner>
        } @else if (filteredWorkflows().length === 0) {
          <app-empty-state
            icon="account_tree"
            title="No workflows found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by creating your first workflow'"
            [actionLabel]="searchQuery() ? undefined : 'Create Workflow'"
            actionIcon="add"
            (action)="router.navigate(['/workflows/new'])"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredWorkflows()" class="workflow-list__table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let workflow">
                <a [routerLink]="['/workflows', workflow.workflowId]" class="workflow-list__link">
                  {{ workflow.name }}
                </a>
              </td>
            </ng-container>

            <ng-container matColumnDef="entityType">
              <th mat-header-cell *matHeaderCellDef>Entity Type</th>
              <td mat-cell *matCellDef="let workflow">{{ workflow.entityType }}</td>
            </ng-container>

            <ng-container matColumnDef="steps">
              <th mat-header-cell *matHeaderCellDef>Steps</th>
              <td mat-cell *matCellDef="let workflow">{{ workflow.steps?.length || 0 }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let workflow">
                <app-status-badge
                  [status]="getStatusType(workflow.status)"
                  [label]="workflow.status"
                ></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let workflow">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/workflows', workflow.workflowId]">
                    <mat-icon>visibility</mat-icon>
                    <span>View</span>
                  </button>
                  <button mat-menu-item [routerLink]="['/workflows', workflow.workflowId, 'edit']">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  @if (workflow.status !== 'Active') {
                    <button mat-menu-item (click)="activateWorkflow(workflow)">
                      <mat-icon>play_arrow</mat-icon>
                      <span>Activate</span>
                    </button>
                  } @else {
                    <button mat-menu-item (click)="deactivateWorkflow(workflow)">
                      <mat-icon>pause</mat-icon>
                      <span>Deactivate</span>
                    </button>
                  }
                  <button mat-menu-item (click)="deleteWorkflow(workflow)" class="workflow-list__delete">
                    <mat-icon>delete</mat-icon>
                    <span>Delete</span>
                  </button>
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
  styles: [`
    .workflow-list {
      &__card {
        overflow: hidden;
      }

      &__toolbar {
        display: flex;
        gap: var(--spacing-md);
        margin-bottom: var(--spacing-md);
      }

      &__search {
        flex: 1;
        max-width: 400px;
      }

      &__table {
        width: 100%;
      }

      &__link {
        color: var(--mat-sys-primary);
        text-decoration: none;
        font-weight: 500;

        &:hover {
          text-decoration: underline;
        }
      }

      &__delete {
        color: var(--mat-sys-error);
      }
    }
  `]
})
export class WorkflowList implements OnInit {
  readonly router = inject(Router);
  private readonly workflowService = inject(WorkflowService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly workflows = signal<Workflow[]>([]);
  readonly searchQuery = signal('');

  readonly displayedColumns = ['name', 'entityType', 'steps', 'status', 'actions'];

  readonly filteredWorkflows = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.workflows();
    return this.workflows().filter(w =>
      w.name.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadWorkflows();
  }

  private loadWorkflows(): void {
    this.workflowService.getAll().subscribe({
      next: (workflows) => {
        this.workflows.set(workflows);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getStatusType(status: WorkflowStatus): 'success' | 'warning' | 'error' | 'info' | 'neutral' {
    switch (status) {
      case WorkflowStatus.Active: return 'success';
      case WorkflowStatus.Inactive: return 'neutral';
      case WorkflowStatus.Draft: return 'info';
      case WorkflowStatus.Deprecated: return 'warning';
      default: return 'neutral';
    }
  }

  activateWorkflow(workflow: Workflow): void {
    this.workflowService.activate(workflow.workflowId).subscribe({
      next: () => {
        this.snackBar.open('Workflow activated successfully', 'Close', { duration: 3000 });
        this.loadWorkflows();
      }
    });
  }

  deactivateWorkflow(workflow: Workflow): void {
    this.workflowService.deactivate(workflow.workflowId).subscribe({
      next: () => {
        this.snackBar.open('Workflow deactivated successfully', 'Close', { duration: 3000 });
        this.loadWorkflows();
      }
    });
  }

  deleteWorkflow(workflow: Workflow): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete Workflow',
        message: `Are you sure you want to delete "${workflow.name}"?`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.workflowService.delete(workflow.workflowId).subscribe({
          next: () => {
            this.snackBar.open('Workflow deleted successfully', 'Close', { duration: 3000 });
            this.loadWorkflows();
          }
        });
      }
    });
  }
}
