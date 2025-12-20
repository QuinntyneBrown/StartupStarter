import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../components/shared';
import { ContentService } from '../../services';
import { ContentType } from '../../models';

@Component({
  selector: 'app-content-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, PageHeaderComponent, LoadingSpinnerComponent],
  template: `
    <app-page-header [title]="isEditMode() ? 'Edit Content' : 'New Content'" icon="article"></app-page-header>
    @if (isLoading()) {
      <app-loading-spinner></app-loading-spinner>
    } @else {
      <mat-card>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Title</mat-label>
              <input matInput formControlName="title">
            </mat-form-field>
            @if (!isEditMode()) {
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Content Type</mat-label>
                <mat-select formControlName="contentType">
                  @for (type of contentTypes; track type) {
                    <mat-option [value]="type">{{ type }}</mat-option>
                  }
                </mat-select>
              </mat-form-field>
            }
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Body</mat-label>
              <textarea matInput formControlName="body" rows="10"></textarea>
            </mat-form-field>
            @if (isEditMode()) {
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Change Description</mat-label>
                <input matInput formControlName="changeDescription" placeholder="Describe your changes">
              </mat-form-field>
            }
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
  styles: [`.full-width { width: 100%; } mat-card { max-width: 800px; } .form-actions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 16px; }`]
})
export class ContentFormComponent implements OnInit {
  private readonly contentService = inject(ContentService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);
  contentTypes = Object.values(ContentType);
  private contentId = '';

  form = this.fb.group({
    title: ['', Validators.required],
    contentType: [ContentType.Article],
    body: ['', Validators.required],
    changeDescription: ['']
  });

  ngOnInit(): void {
    this.contentId = this.route.snapshot.paramMap.get('id') || '';
    if (this.contentId && this.contentId !== 'new') {
      this.isEditMode.set(true);
      this.loadContent();
    }
  }

  loadContent(): void {
    this.isLoading.set(true);
    this.contentService.getById(this.contentId).subscribe({
      next: (content) => { this.form.patchValue({ title: content.title, body: content.body }); this.isLoading.set(false); },
      error: () => { this.isLoading.set(false); this.router.navigate(['/content']); }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSaving.set(true);
    const { title, contentType, body, changeDescription } = this.form.value;

    if (this.isEditMode()) {
      this.contentService.update(this.contentId, { title: title!, body: body!, changeDescription: changeDescription! }).subscribe({
        next: () => this.router.navigate(['/content']),
        error: () => this.isSaving.set(false)
      });
    } else {
      this.contentService.create({ title: title!, contentType: contentType!, body: body! }).subscribe({
        next: () => this.router.navigate(['/content']),
        error: () => this.isSaving.set(false)
      });
    }
  }

  cancel(): void { this.router.navigate(['/content']); }
}
