import { Component, Input, Output, EventEmitter } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [MatIconModule, MatButtonModule],
  template: `
    <div class="empty-state">
      @if (icon) {
        <mat-icon class="empty-icon">{{ icon }}</mat-icon>
      }
      <h3 class="empty-title">{{ title }}</h3>
      @if (message) {
        <p class="empty-message">{{ message }}</p>
      }
      @if (actionLabel) {
        <button mat-flat-button color="primary" (click)="action.emit()">
          {{ actionLabel }}
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
      padding: 48px;
      text-align: center;
    }

    .empty-icon {
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: rgba(0, 0, 0, 0.26);
      margin-bottom: 16px;
    }

    .empty-title {
      margin: 0 0 8px 0;
      font-size: 18px;
      font-weight: 500;
      color: rgba(0, 0, 0, 0.87);
    }

    .empty-message {
      margin: 0 0 24px 0;
      font-size: 14px;
      color: rgba(0, 0, 0, 0.6);
      max-width: 400px;
    }
  `]
})
export class EmptyStateComponent {
  @Input() icon = '';
  @Input() title = 'No items found';
  @Input() message = '';
  @Input() actionLabel = '';
  @Output() action = new EventEmitter<void>();
}
