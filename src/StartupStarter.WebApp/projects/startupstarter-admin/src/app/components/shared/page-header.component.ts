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
    :host {
      display: block;
      margin: -24px -24px 24px -24px;
    }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px 32px;
      background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%);
      color: white;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      flex-wrap: wrap;
      gap: 16px;
    }

    .page-header-content {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .page-icon {
      font-size: 36px;
      width: 36px;
      height: 36px;
      color: white;
      opacity: 0.9;
    }

    .page-title-container {
      display: flex;
      flex-direction: column;
    }

    .page-title {
      margin: 0;
      font-size: 28px;
      font-weight: 500;
      color: white;
    }

    .page-subtitle {
      margin: 4px 0 0 0;
      font-size: 14px;
      color: white;
      opacity: 0.9;
    }

    .page-actions {
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
    }

    .page-actions ::ng-deep button.mat-mdc-unelevated-button {
      background-color: white !important;
      color: #1976d2 !important;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .page-actions ::ng-deep button.mat-mdc-unelevated-button:hover {
      background-color: #f5f5f5 !important;
    }
  `]
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() subtitle = '';
  @Input() icon = '';
}
