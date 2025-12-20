import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeader } from '../../components/shared/page-header';
import { AccountService, UserService, ContentService, WorkflowService } from '../../services';
import { forkJoin } from 'rxjs';

interface DashboardCard {
  title: string;
  value: number | string;
  icon: string;
  color: string;
  route: string;
  trend?: { value: number; direction: 'up' | 'down' };
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    RouterLink,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    PageHeader
  ],
  template: `
    <app-page-header
      title="Dashboard"
      subtitle="Welcome back! Here's an overview of your system."
      icon="dashboard"
    ></app-page-header>

    @if (isLoading()) {
      <div class="dashboard__loading">
        <mat-spinner diameter="48"></mat-spinner>
      </div>
    } @else {
      <div class="dashboard__grid">
        @for (card of cards(); track card.title) {
          <mat-card class="dashboard__card" [routerLink]="card.route">
            <mat-card-content class="dashboard__card-content">
              <div class="dashboard__card-icon" [style.background]="card.color">
                <mat-icon>{{ card.icon }}</mat-icon>
              </div>
              <div class="dashboard__card-info">
                <span class="dashboard__card-value text-headline-medium">{{ card.value }}</span>
                <span class="dashboard__card-title text-body-medium">{{ card.title }}</span>
              </div>
              @if (card.trend) {
                <div class="dashboard__card-trend" [class.dashboard__card-trend--up]="card.trend.direction === 'up'" [class.dashboard__card-trend--down]="card.trend.direction === 'down'">
                  <mat-icon>{{ card.trend.direction === 'up' ? 'trending_up' : 'trending_down' }}</mat-icon>
                  <span>{{ card.trend.value }}%</span>
                </div>
              }
            </mat-card-content>
          </mat-card>
        }
      </div>

      <div class="dashboard__sections">
        <mat-card class="dashboard__section">
          <mat-card-header>
            <mat-card-title>Recent Activity</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="dashboard__activity-list">
              @for (activity of recentActivity(); track activity.id) {
                <div class="dashboard__activity-item">
                  <mat-icon class="dashboard__activity-icon">{{ activity.icon }}</mat-icon>
                  <div class="dashboard__activity-content">
                    <span class="text-body-medium">{{ activity.message }}</span>
                    <span class="text-body-small dashboard__activity-time">{{ activity.time }}</span>
                  </div>
                </div>
              }
            </div>
          </mat-card-content>
        </mat-card>

        <mat-card class="dashboard__section">
          <mat-card-header>
            <mat-card-title>Pending Approvals</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="dashboard__approvals">
              @if (pendingWorkflows().length === 0) {
                <div class="dashboard__empty">
                  <mat-icon>check_circle</mat-icon>
                  <span class="text-body-medium">No pending approvals</span>
                </div>
              } @else {
                @for (workflow of pendingWorkflows(); track workflow.workflowId) {
                  <div class="dashboard__approval-item">
                    <div class="dashboard__approval-info">
                      <span class="text-body-medium">{{ workflow.workflowType }}</span>
                      <span class="text-body-small">Stage: {{ workflow.currentStage }}</span>
                    </div>
                    <button mat-stroked-button color="primary" [routerLink]="['/workflows', workflow.workflowId]">
                      Review
                    </button>
                  </div>
                }
              }
            </div>
          </mat-card-content>
        </mat-card>
      </div>
    }
  `,
  styles: [`
    .dashboard {
      &__loading {
        display: flex;
        justify-content: center;
        padding: var(--spacing-3xl);
      }

      &__grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
        gap: var(--spacing-md);
        margin-bottom: var(--spacing-xl);
      }

      &__card {
        cursor: pointer;
        transition: transform var(--transition-fast), box-shadow var(--transition-fast);

        &:hover {
          transform: translateY(-2px);
          box-shadow: var(--shadow-lg);
        }
      }

      &__card-content {
        display: flex;
        align-items: center;
        gap: var(--spacing-md);
        padding: var(--spacing-lg) !important;
      }

      &__card-icon {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 56px;
        height: 56px;
        border-radius: var(--radius-lg);
        color: white;

        mat-icon {
          font-size: 28px;
          width: 28px;
          height: 28px;
        }
      }

      &__card-info {
        display: flex;
        flex-direction: column;
        flex: 1;
      }

      &__card-value {
        color: var(--mat-sys-on-surface);
      }

      &__card-title {
        color: var(--mat-sys-on-surface-variant);
      }

      &__card-trend {
        display: flex;
        align-items: center;
        gap: var(--spacing-xs);
        font: var(--mat-sys-label-small);

        &--up {
          color: var(--mat-sys-primary);
        }

        &--down {
          color: var(--mat-sys-error);
        }

        mat-icon {
          font-size: 18px;
          width: 18px;
          height: 18px;
        }
      }

      &__sections {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
        gap: var(--spacing-md);
      }

      &__section {
        min-height: 300px;
      }

      &__activity-list {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-md);
      }

      &__activity-item {
        display: flex;
        align-items: flex-start;
        gap: var(--spacing-md);
        padding: var(--spacing-sm) 0;
        border-bottom: 1px solid var(--mat-sys-outline-variant);

        &:last-child {
          border-bottom: none;
        }
      }

      &__activity-icon {
        color: var(--mat-sys-primary);
      }

      &__activity-content {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-xs);
      }

      &__activity-time {
        color: var(--mat-sys-on-surface-variant);
      }

      &__approvals {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-md);
      }

      &__approval-item {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: var(--spacing-md);
        padding: var(--spacing-sm);
        background: var(--mat-sys-surface-container);
        border-radius: var(--radius-md);
      }

      &__approval-info {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-xs);
      }

      &__empty {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: var(--spacing-md);
        padding: var(--spacing-xl);
        color: var(--mat-sys-on-surface-variant);

        mat-icon {
          font-size: 48px;
          width: 48px;
          height: 48px;
        }
      }
    }
  `]
})
export class Dashboard implements OnInit {
  private readonly accountService = inject(AccountService);
  private readonly userService = inject(UserService);
  private readonly contentService = inject(ContentService);
  private readonly workflowService = inject(WorkflowService);

  readonly isLoading = signal(true);
  readonly cards = signal<DashboardCard[]>([]);
  readonly pendingWorkflows = signal<any[]>([]);
  readonly recentActivity = signal<{ id: string; icon: string; message: string; time: string }[]>([
    { id: '1', icon: 'person_add', message: 'New user registered: john.doe@example.com', time: '5 minutes ago' },
    { id: '2', icon: 'article', message: 'Content published: Getting Started Guide', time: '1 hour ago' },
    { id: '3', icon: 'check_circle', message: 'Workflow approved: Marketing Campaign', time: '2 hours ago' },
    { id: '4', icon: 'security', message: 'Role updated: Editor permissions changed', time: '3 hours ago' },
    { id: '5', icon: 'backup', message: 'System backup completed successfully', time: '6 hours ago' }
  ]);

  ngOnInit(): void {
    this.loadDashboardData();
  }

  private loadDashboardData(): void {
    forkJoin({
      accounts: this.accountService.getAll(),
      users: this.userService.getAll(),
      content: this.contentService.getAll(),
      workflows: this.workflowService.getPending()
    }).subscribe({
      next: ({ accounts, users, content, workflows }) => {
        this.cards.set([
          {
            title: 'Total Accounts',
            value: accounts.length,
            icon: 'business',
            color: 'var(--mat-sys-primary)',
            route: '/accounts',
            trend: { value: 12, direction: 'up' }
          },
          {
            title: 'Active Users',
            value: users.filter(u => u.status === 'Active').length,
            icon: 'people',
            color: 'var(--mat-sys-tertiary)',
            route: '/users',
            trend: { value: 8, direction: 'up' }
          },
          {
            title: 'Published Content',
            value: content.filter(c => c.status === 'Published').length,
            icon: 'article',
            color: 'var(--mat-sys-secondary)',
            route: '/content'
          },
          {
            title: 'Pending Approvals',
            value: workflows.length,
            icon: 'pending_actions',
            color: workflows.length > 0 ? 'var(--mat-sys-error)' : 'var(--mat-sys-outline)',
            route: '/workflows'
          }
        ]);
        this.pendingWorkflows.set(workflows.slice(0, 5));
        this.isLoading.set(false);
      },
      error: () => {
        this.cards.set([
          { title: 'Total Accounts', value: '-', icon: 'business', color: 'var(--mat-sys-primary)', route: '/accounts' },
          { title: 'Active Users', value: '-', icon: 'people', color: 'var(--mat-sys-tertiary)', route: '/users' },
          { title: 'Published Content', value: '-', icon: 'article', color: 'var(--mat-sys-secondary)', route: '/content' },
          { title: 'Pending Approvals', value: '-', icon: 'pending_actions', color: 'var(--mat-sys-outline)', route: '/workflows' }
        ]);
        this.isLoading.set(false);
      }
    });
  }
}
