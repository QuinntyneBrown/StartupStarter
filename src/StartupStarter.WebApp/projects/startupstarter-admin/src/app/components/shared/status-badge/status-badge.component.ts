import { Component, Input } from '@angular/core';

export type BadgeType = 'success' | 'warning' | 'error' | 'info' | 'neutral';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  templateUrl: './status-badge.component.html',
  styleUrl: './status-badge.component.scss'
})
export class StatusBadgeComponent {
  @Input() label = '';
  @Input() type: BadgeType = 'neutral';
}
