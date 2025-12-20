import { Component, inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface ConfirmDialogData {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  confirmColor?: 'primary' | 'accent' | 'warn';
  icon?: string;
}

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <div class="confirm-dialog">
      @if (data.icon) {
        <mat-icon class="confirm-dialog__icon" [class.confirm-dialog__icon--warn]="data.confirmColor === 'warn'">
          {{ data.icon }}
        </mat-icon>
      }

      <h2 mat-dialog-title class="confirm-dialog__title text-headline-small">{{ data.title }}</h2>

      <mat-dialog-content class="confirm-dialog__content">
        <p class="text-body-medium">{{ data.message }}</p>
      </mat-dialog-content>

      <mat-dialog-actions align="end" class="confirm-dialog__actions">
        <button mat-button (click)="onCancel()">
          {{ data.cancelText || 'Cancel' }}
        </button>
        <button mat-raised-button [color]="data.confirmColor || 'primary'" (click)="onConfirm()">
          {{ data.confirmText || 'Confirm' }}
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .confirm-dialog {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      padding: var(--spacing-md);

      &__icon {
        font-size: 48px;
        width: 48px;
        height: 48px;
        margin-bottom: var(--spacing-md);
        color: var(--mat-sys-primary);

        &--warn {
          color: var(--mat-sys-error);
        }
      }

      &__title {
        margin: 0 0 var(--spacing-md) 0;
      }

      &__content {
        margin-bottom: var(--spacing-md);
      }

      &__actions {
        width: 100%;
        padding: 0;
      }
    }
  `]
})
export class ConfirmDialog {
  private readonly dialogRef = inject(MatDialogRef<ConfirmDialog>);
  readonly data: ConfirmDialogData = inject(MAT_DIALOG_DATA);

  onConfirm(): void {
    this.dialogRef.close(true);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
