import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../components/shared';
import { WebhookService } from '../../services';
import { WEBHOOK_EVENTS } from '../../models';

@Component({
  selector: 'app-webhook-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, PageHeaderComponent, LoadingSpinnerComponent],
  template: `
    <app-page-header [title]="isEditMode() ? 'Edit Webhook' : 'New Webhook'" icon="webhook"></app-page-header>
    @if (isLoading()) {
      <app-loading-spinner></app-loading-spinner>
    } @else {
      <mat-card>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Webhook URL</mat-label>
              <input matInput formControlName="url" placeholder="https://your-server.com/webhook">
              @if (form.get('url')?.hasError('pattern')) {
                <mat-error>URL must be HTTPS</mat-error>
              }
            </mat-form-field>
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Events</mat-label>
              <mat-select formControlName="events" multiple>
                @for (event of webhookEvents; track event) {
                  <mat-option [value]="event">{{ event }}</mat-option>
                }
              </mat-select>
            </mat-form-field>
            <div class="form-actions">
              <button mat-button type="button" (click)="cancel()">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="isSaving() || form.invalid">
                {{ isSaving() ? 'Saving...' : (isEditMode() ? 'Update' : 'Create') }}
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    }
  `,
  styles: [`.full-width { width: 100%; } mat-card { max-width: 600px; } .form-actions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 16px; }`]
})
export class WebhookFormComponent implements OnInit {
  private readonly webhookService = inject(WebhookService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);
  webhookEvents = WEBHOOK_EVENTS;
  private webhookId = '';

  form = this.fb.group({
    url: ['', [Validators.required, Validators.pattern(/^https:\/\/.+/)]],
    events: [[] as string[], Validators.required]
  });

  ngOnInit(): void {
    this.webhookId = this.route.snapshot.paramMap.get('id') || '';
    if (this.webhookId && this.webhookId !== 'new') {
      this.isEditMode.set(true);
      this.loadWebhook();
    }
  }

  loadWebhook(): void {
    this.isLoading.set(true);
    this.webhookService.getById(this.webhookId).subscribe({
      next: (webhook) => { this.form.patchValue({ url: webhook.url, events: webhook.events }); this.isLoading.set(false); },
      error: () => { this.isLoading.set(false); this.router.navigate(['/webhooks']); }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSaving.set(true);
    const { url, events } = this.form.value;

    if (this.isEditMode()) {
      this.webhookService.update(this.webhookId, { url: url!, events: events! }).subscribe({
        next: () => this.router.navigate(['/webhooks']),
        error: () => this.isSaving.set(false)
      });
    } else {
      this.webhookService.create({ url: url!, events: events! }).subscribe({
        next: () => this.router.navigate(['/webhooks']),
        error: () => this.isSaving.set(false)
      });
    }
  }

  cancel(): void { this.router.navigate(['/webhooks']); }
}
