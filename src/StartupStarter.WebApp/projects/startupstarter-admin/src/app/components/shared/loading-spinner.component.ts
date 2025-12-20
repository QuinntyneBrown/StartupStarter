import { Component, Input } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [MatProgressSpinnerModule],
  template: `
    <div class="loading-spinner">
      <mat-spinner [diameter]="diameter"></mat-spinner>
      @if (message) {
        <p class="loading-spinner__message">{{ message }}</p>
      }
    </div>
  `,
  styles: [`
    .loading-spinner {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 48px;
      gap: 16px;
    }

    .loading-spinner__message {
      color: rgba(0, 0, 0, 0.6);
      font-size: 14px;
      margin: 0;
    }
  `]
})
export class LoadingSpinnerComponent {
  @Input() diameter = 48;
  @Input() message = '';
}
