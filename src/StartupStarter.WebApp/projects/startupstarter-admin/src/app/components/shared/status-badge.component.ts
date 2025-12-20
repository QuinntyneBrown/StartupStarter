import { Component, Input } from '@angular/core';

export type BadgeType = 'success' | 'warning' | 'error' | 'info' | 'neutral';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `
    <span class="status-badge" [class]="'status-badge--' + type">
      {{ label }}
    </span>
  `,
  styles: [`
    .status-badge {
      display: inline-flex;
      align-items: center;
      padding: 4px 12px;
      border-radius: 16px;
      font-size: 12px;
      font-weight: 500;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .status-badge--success {
      background-color: #e8f5e9;
      color: #2e7d32;
    }

    .status-badge--warning {
      background-color: #fff3e0;
      color: #ef6c00;
    }

    .status-badge--error {
      background-color: #ffebee;
      color: #c62828;
    }

    .status-badge--info {
      background-color: #e3f2fd;
      color: #1565c0;
    }

    .status-badge--neutral {
      background-color: #f5f5f5;
      color: #616161;
    }
  `]
})
export class StatusBadgeComponent {
  @Input() label = '';
  @Input() type: BadgeType = 'neutral';
}
