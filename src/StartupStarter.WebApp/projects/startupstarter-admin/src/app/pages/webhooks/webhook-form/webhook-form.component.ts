import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../../components/shared';
import { WebhookService } from '../../../services';
import { WEBHOOK_EVENTS } from '../../../models';

@Component({
  selector: 'app-webhook-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, PageHeaderComponent, LoadingSpinnerComponent],
  templateUrl: './webhook-form.component.html',
  styleUrl: './webhook-form.component.scss'
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
