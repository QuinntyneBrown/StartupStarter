import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [MatIconModule],
  template: `
    <div class="page-header">
      <div class="page-header-content">
        @if (icon) {
          <mat-icon class="page-icon">{{ icon }}</mat-icon>
        }
        <div class="page-title-container">
          <h1 class="page-title">{{ title }}</h1>
          @if (subtitle) {
            <p class="page-subtitle">{{ subtitle }}</p>
          }
        </div>
      </div>
      <div class="page-actions">
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

    .page-header-content {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .page-icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
      color: var(--mat-primary-color, #3f51b5);
    }

    .page-title-container {
      display: flex;
      flex-direction: column;
    }

    .page-title {
      margin: 0;
      font-size: 24px;
      font-weight: 500;
      color: rgba(0, 0, 0, 0.87);
    }

    .page-subtitle {
      margin: 4px 0 0 0;
      font-size: 14px;
      color: rgba(0, 0, 0, 0.6);
    }

    .page-actions {
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
