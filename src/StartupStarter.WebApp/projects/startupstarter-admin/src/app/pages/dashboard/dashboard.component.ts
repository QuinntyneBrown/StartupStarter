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
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
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
