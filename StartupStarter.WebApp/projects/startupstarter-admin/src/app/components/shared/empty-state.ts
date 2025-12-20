import { Component, input, output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [MatIconModule, MatButtonModule],
  template: `
    <div class="empty-state">
      <mat-icon class="empty-state__icon">{{ icon() }}</mat-icon>
      <h3 class="empty-state__title text-title-large">{{ title() }}</h3>
      @if (message()) {
        <p class="empty-state__message text-body-medium">{{ message() }}</p>
      }
      @if (actionLabel()) {
        <button mat-raised-button color="primary" (click)="action.emit()">
          @if (actionIcon()) {
            <mat-icon>{{ actionIcon() }}</mat-icon>
          }
          {{ actionLabel() }}
        </button>
      }
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      text-align: center;
      padding: var(--spacing-3xl);
      gap: var(--spacing-md);

      &__icon {
        font-size: 64px;
        width: 64px;
        height: 64px;
        color: var(--mat-sys-outline);
      }

      &__title {
        margin: 0;
        color: var(--mat-sys-on-surface);
      }

      &__message {
        margin: 0;
        color: var(--mat-sys-on-surface-variant);
        max-width: 400px;
      }
    }
  `]
})
export class EmptyState {
  readonly icon = input('inbox');
  readonly title = input.required<string>();
  readonly message = input<string>();
  readonly actionLabel = input<string>();
  readonly actionIcon = input<string>();

  readonly action = output<void>();
}
