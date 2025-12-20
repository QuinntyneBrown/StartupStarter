import { Component, input, computed } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';

type StatusType = 'success' | 'warning' | 'error' | 'info' | 'neutral';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [MatChipsModule],
  template: `
    <span class="status-badge" [class]="badgeClass()">
      {{ label() }}
    </span>
  `,
  styles: [`
    .status-badge {
      display: inline-flex;
      align-items: center;
      padding: var(--spacing-xs) var(--spacing-sm);
      border-radius: var(--radius-full);
      font: var(--mat-sys-label-small);
      text-transform: uppercase;
      letter-spacing: 0.5px;

      &--success {
        background: color-mix(in srgb, var(--mat-sys-primary) 20%, transparent);
        color: var(--mat-sys-primary);
      }

      &--warning {
        background: color-mix(in srgb, var(--mat-sys-tertiary) 20%, transparent);
        color: var(--mat-sys-tertiary);
      }

      &--error {
        background: var(--mat-sys-error-container);
        color: var(--mat-sys-on-error-container);
      }

      &--info {
        background: color-mix(in srgb, var(--mat-sys-secondary) 20%, transparent);
        color: var(--mat-sys-secondary);
      }

      &--neutral {
        background: var(--mat-sys-surface-container-high);
        color: var(--mat-sys-on-surface-variant);
      }
    }
  `]
})
export class StatusBadge {
  readonly status = input.required<StatusType>();
  readonly label = input.required<string>();

  readonly badgeClass = computed(() => `status-badge status-badge--${this.status()}`);
}
