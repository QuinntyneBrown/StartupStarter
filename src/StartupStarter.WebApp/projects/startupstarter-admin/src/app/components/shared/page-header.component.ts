import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [MatIconModule],
  template: `
    <div class="page-header">
      <div class="page-header__content">
        @if (icon) {
          <mat-icon class="page-header__icon">{{ icon }}</mat-icon>
        }
        <div class="page-header__title-container">
          <h1 class="page-header__title">{{ title }}</h1>
          @if (subtitle) {
            <p class="page-header__subtitle">{{ subtitle }}</p>
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
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
      flex-wrap: wrap;
      gap: 16px;
    }

    .page-header__content {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .page-header__icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
      color: var(--mat-primary-color, #3f51b5);
    }

    .page-header__title-container {
      display: flex;
      flex-direction: column;
    }

    .page-header__title {
      margin: 0;
      font-size: 24px;
      font-weight: 500;
      color: rgba(0, 0, 0, 0.87);
    }

    .page-header__subtitle {
      margin: 4px 0 0 0;
      font-size: 14px;
      color: rgba(0, 0, 0, 0.6);
    }

    .page-header__actions {
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
    }
  `]
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() subtitle = '';
  @Input() icon = '';
}
