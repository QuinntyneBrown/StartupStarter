import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [MatIconModule, MatButtonModule],
  template: `
    <div class="page-header">
      <div class="page-header__left">
        @if (icon()) {
          <mat-icon class="page-header__icon">{{ icon() }}</mat-icon>
        }
        <div class="page-header__text">
          <h1 class="page-header__title text-headline-medium">{{ title() }}</h1>
          @if (subtitle()) {
            <p class="page-header__subtitle text-body-medium">{{ subtitle() }}</p>
          }
        </div>
      </div>
      <div class="page-header__actions">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      flex-wrap: wrap;
      align-items: flex-start;
      justify-content: space-between;
      gap: var(--spacing-md);
      margin-bottom: var(--spacing-lg);

      &__left {
        display: flex;
        align-items: flex-start;
        gap: var(--spacing-md);
      }

      &__icon {
        font-size: 32px;
        width: 32px;
        height: 32px;
        color: var(--mat-sys-primary);
        margin-top: var(--spacing-xs);
      }

      &__text {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-xs);
      }

      &__title {
        margin: 0;
        color: var(--mat-sys-on-surface);
      }

      &__subtitle {
        margin: 0;
        color: var(--mat-sys-on-surface-variant);
      }

      &__actions {
        display: flex;
        gap: var(--spacing-sm);
        flex-wrap: wrap;
      }
    }

    @media (max-width: 768px) {
      .page-header {
        flex-direction: column;
        align-items: stretch;

        &__actions {
          justify-content: flex-start;
        }
      }
    }
  `]
})
export class PageHeader {
  readonly title = input.required<string>();
  readonly subtitle = input<string>();
  readonly icon = input<string>();
}
