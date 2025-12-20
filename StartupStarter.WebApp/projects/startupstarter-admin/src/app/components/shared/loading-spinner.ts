import { Component, input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [MatProgressSpinnerModule],
  template: `
    <div class="loading-spinner" [class.loading-spinner--overlay]="overlay()">
      <mat-spinner [diameter]="diameter()"></mat-spinner>
      @if (message()) {
        <p class="loading-spinner__message text-body-medium">{{ message() }}</p>
      }
    </div>
  `,
  styles: [`
    .loading-spinner {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: var(--spacing-md);
      padding: var(--spacing-xl);

      &--overlay {
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(var(--mat-sys-surface), 0.8);
        z-index: 100;
      }

      &__message {
        color: var(--mat-sys-on-surface-variant);
        margin: 0;
      }
    }
  `]
})
export class LoadingSpinner {
  readonly diameter = input(48);
  readonly message = input<string>();
  readonly overlay = input(false);
}
