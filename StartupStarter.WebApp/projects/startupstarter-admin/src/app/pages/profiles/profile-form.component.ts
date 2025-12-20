import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../components/shared';
import { ProfileService } from '../../services';
import { ProfileType } from '../../models';

@Component({
  selector: 'app-profile-form',
  standalone: true,
  imports: [ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule, PageHeaderComponent, LoadingSpinnerComponent],
  template: `
    <app-page-header [title]="isEditMode() ? 'Edit Profile' : 'New Profile'" icon="person"></app-page-header>
    @if (isLoading()) {
      <app-loading-spinner></app-loading-spinner>
    } @else {
      <mat-card>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Profile Name</mat-label>
              <input matInput formControlName="profileName">
            </mat-form-field>
            @if (!isEditMode()) {
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Profile Type</mat-label>
                <mat-select formControlName="profileType">
                  @for (type of profileTypes; track type) {
                    <mat-option [value]="type">{{ type }}</mat-option>
                  }
                </mat-select>
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
  styles: [`.full-width { width: 100%; } mat-card { max-width: 600px; } .form-actions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 16px; }`]
})
export class ProfileFormComponent implements OnInit {
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);
  profileTypes = Object.values(ProfileType);
  private profileId = '';

  form = this.fb.group({
    profileName: ['', Validators.required],
    profileType: [ProfileType.Personal]
  });

  ngOnInit(): void {
    this.profileId = this.route.snapshot.paramMap.get('id') || '';
    if (this.profileId && this.profileId !== 'new') {
      this.isEditMode.set(true);
      this.loadProfile();
    }
  }

  loadProfile(): void {
    this.isLoading.set(true);
    this.profileService.getById(this.profileId).subscribe({
      next: (profile) => { this.form.patchValue({ profileName: profile.profileName }); this.isLoading.set(false); },
      error: () => { this.isLoading.set(false); this.router.navigate(['/profiles']); }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSaving.set(true);
    const { profileName, profileType } = this.form.value;

    if (this.isEditMode()) {
      this.profileService.update(this.profileId, { profileName: profileName! }).subscribe({
        next: () => this.router.navigate(['/profiles']),
        error: () => this.isSaving.set(false)
      });
    } else {
      this.profileService.create({ profileName: profileName!, profileType: profileType! }).subscribe({
        next: () => this.router.navigate(['/profiles']),
        error: () => this.isSaving.set(false)
      });
    }
  }

  cancel(): void { this.router.navigate(['/profiles']); }
}
