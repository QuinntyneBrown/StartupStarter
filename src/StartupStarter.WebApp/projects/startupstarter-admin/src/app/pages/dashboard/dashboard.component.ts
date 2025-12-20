import { Component, inject, signal, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { PageHeaderComponent } from '../../components/shared';
import { LoadingSpinnerComponent } from '../../components/shared';
import { AuthService } from '../../services';

interface DashboardCard {
  title: string;
  value: string | number;
  icon: string;
  color: string;
  route?: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    RouterLink,
    PageHeaderComponent,
    LoadingSpinnerComponent
  ],
  template: `
    <app-page-header
      title="Dashboard"
      subtitle="Welcome back, {{ userName() }}"
      icon="dashboard">
    </app-page-header>

    @if (isLoading()) {
      <app-loading-spinner message="Loading dashboard..."></app-loading-spinner>
    } @else {
      <div class="dashboard__grid">
        @for (card of cards(); track card.title) {
          <mat-card class="dashboard__card" [style.border-left-color]="card.color">
            <mat-card-content>
              <div class="dashboard__card-icon" [style.background-color]="card.color + '20'" [style.color]="card.color">
                <mat-icon>{{ card.icon }}</mat-icon>
              </div>
              <div class="dashboard__card-content">
                <h3 class="dashboard__card-value">{{ card.value }}</h3>
                <p class="dashboard__card-title">{{ card.title }}</p>
              </div>
              @if (card.route) {
                <a mat-icon-button [routerLink]="card.route" class="dashboard__card-action">
                  <mat-icon>arrow_forward</mat-icon>
                </a>
              }
            </mat-card-content>
          </mat-card>
        }
      </div>

      <div class="dashboard__sections">
        <mat-card class="dashboard__section-card">
          <mat-card-header>
            <mat-card-title>Recent Activity</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="dashboard__activity-list">
              @for (activity of recentActivity(); track activity.id) {
                <div class="dashboard__activity-item">
                  <mat-icon [style.color]="activity.color">{{ activity.icon }}</mat-icon>
                  <div class="dashboard__activity-content">
                    <span class="dashboard__activity-message">{{ activity.message }}</span>
                    <span class="dashboard__activity-time">{{ activity.time }}</span>
                  </div>
                </div>
              }
            </div>
          </mat-card-content>
        </mat-card>

        <mat-card class="dashboard__section-card">
          <mat-card-header>
            <mat-card-title>Quick Actions</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="dashboard__quick-actions">
              <button mat-stroked-button routerLink="/users">
                <mat-icon>person_add</mat-icon>
                Invite User
              </button>
              <button mat-stroked-button routerLink="/content">
                <mat-icon>add_circle</mat-icon>
                Create Content
              </button>
              <button mat-stroked-button routerLink="/workflows">
                <mat-icon>account_tree</mat-icon>
                View Workflows
              </button>
              <button mat-stroked-button routerLink="/audit">
                <mat-icon>history</mat-icon>
                Audit Logs
              </button>
            </div>
          </mat-card-content>
        </mat-card>
      </div>
    }
  `,
  styles: [`
    .dashboard__grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 24px;
      margin-bottom: 24px;
    }

    .dashboard__card {
      border-left: 4px solid;
    }

    .dashboard__card mat-card-content {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 24px !important;
    }

    .dashboard__card-icon {
      width: 56px;
      height: 56px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .dashboard__card-icon mat-icon {
      font-size: 28px;
      width: 28px;
      height: 28px;
    }

    .dashboard__card-content {
      flex: 1;
    }

    .dashboard__card-value {
      font-size: 28px;
      font-weight: 600;
      margin: 0;
      color: rgba(0, 0, 0, 0.87);
    }

    .dashboard__card-title {
      font-size: 14px;
      color: rgba(0, 0, 0, 0.6);
      margin: 4px 0 0 0;
    }

    .dashboard__sections {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
      gap: 24px;
    }

    .dashboard__section-card {
      padding: 8px;
    }

    .dashboard__activity-list {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .dashboard__activity-item {
      display: flex;
      align-items: flex-start;
      gap: 12px;
    }

    .dashboard__activity-content {
      display: flex;
      flex-direction: column;
    }

    .dashboard__activity-message {
      font-size: 14px;
      color: rgba(0, 0, 0, 0.87);
    }

    .dashboard__activity-time {
      font-size: 12px;
      color: rgba(0, 0, 0, 0.6);
    }

    .dashboard__quick-actions {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 12px;
    }

    .dashboard__quick-actions button {
      justify-content: flex-start;
      padding: 12px 16px;
    }

    .dashboard__quick-actions mat-icon {
      margin-right: 8px;
    }
  `]
})
export class DashboardComponent implements OnInit {
  private readonly authService = inject(AuthService);

  readonly isLoading = signal(true);
  readonly userName = signal('');

  readonly cards = signal<DashboardCard[]>([
    { title: 'Total Users', value: 0, icon: 'people', color: '#3f51b5', route: '/users' },
    { title: 'Active Content', value: 0, icon: 'article', color: '#4caf50', route: '/content' },
    { title: 'Pending Approvals', value: 0, icon: 'pending_actions', color: '#ff9800', route: '/workflows' },
    { title: 'Media Files', value: 0, icon: 'perm_media', color: '#9c27b0', route: '/media' }
  ]);

  readonly recentActivity = signal<Array<{ id: number; icon: string; color: string; message: string; time: string }>>([
    { id: 1, icon: 'person_add', color: '#4caf50', message: 'New user registered', time: '5 minutes ago' },
    { id: 2, icon: 'article', color: '#3f51b5', message: 'Content published', time: '1 hour ago' },
    { id: 3, icon: 'check_circle', color: '#4caf50', message: 'Workflow approved', time: '2 hours ago' },
    { id: 4, icon: 'upload', color: '#9c27b0', message: 'Media uploaded', time: '3 hours ago' },
    { id: 5, icon: 'security', color: '#ff9800', message: 'Security settings updated', time: 'Yesterday' }
  ]);

  ngOnInit(): void {
    const user = this.authService.user();
    this.userName.set(user?.firstName || 'User');

    setTimeout(() => {
      this.cards.set([
        { title: 'Total Users', value: 24, icon: 'people', color: '#3f51b5', route: '/users' },
        { title: 'Active Content', value: 156, icon: 'article', color: '#4caf50', route: '/content' },
        { title: 'Pending Approvals', value: 8, icon: 'pending_actions', color: '#ff9800', route: '/workflows' },
        { title: 'Media Files', value: 342, icon: 'perm_media', color: '#9c27b0', route: '/media' }
      ]);
      this.isLoading.set(false);
    }, 500);
  }
}
