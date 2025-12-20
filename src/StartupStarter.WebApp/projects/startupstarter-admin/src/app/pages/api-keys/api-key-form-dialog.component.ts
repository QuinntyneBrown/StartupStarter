import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { ApiKeyService } from '../../services';
import { SYSTEM_PERMISSIONS } from '../../models';

@Component({
  selector: 'app-api-key-form-dialog',
  standalone: true,
  imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>Create API Key</h2>
    <mat-dialog-content>
      <form [formGroup]="form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Key Name</mat-label>
          <input matInput formControlName="keyName" placeholder="My API Key">
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Permissions</mat-label>
          <mat-select formControlName="permissions" multiple>
            @for (perm of permissions; track perm) {
              <mat-option [value]="perm">{{ perm }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="cancel()">Cancel</button>
      <button mat-flat-button color="primary" (click)="submit()" [disabled]="isSaving() || form.invalid">
        {{ isSaving() ? 'Creating...' : 'Create' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.full-width { width: 100%; }`]
})
export class ApiKeyFormDialogComponent {
  private readonly apiKeyService = inject(ApiKeyService);
  private readonly dialogRef = inject(MatDialogRef<ApiKeyFormDialogComponent>);
  private readonly fb = inject(FormBuilder);

  readonly isSaving = signal(false);
  permissions = SYSTEM_PERMISSIONS;

  form = this.fb.group({
    keyName: ['', Validators.required],
    permissions: [[] as string[]]
  });

  submit(): void {
    if (this.form.invalid) return;
    this.isSaving.set(true);
    const { keyName, permissions } = this.form.value;
    this.apiKeyService.create({ keyName: keyName!, permissions: permissions! }).subscribe({
      next: (result) => this.dialogRef.close(result),
      error: () => this.isSaving.set(false)
    });
  }

  cancel(): void { this.dialogRef.close(); }
}
