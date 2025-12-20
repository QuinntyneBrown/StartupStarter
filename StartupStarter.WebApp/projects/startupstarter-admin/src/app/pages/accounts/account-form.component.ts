import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../components/shared';
import { AccountService } from '../../services';
import { AccountType } from '../../models';

@Component({
  selector: 'app-account-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    PageHeaderComponent,
    LoadingSpinnerComponent
  ],
  template: `
    <app-page-header
      [title]="isEditMode() ? 'Edit Account' : 'New Account'"
      [subtitle]="isEditMode() ? 'Update account details' : 'Create a new tenant account'"
      icon="business">
    </app-page-header>

    @if (isLoading()) {
      <app-loading-spinner></app-loading-spinner>
    } @else {
      <mat-card>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Account Name</mat-label>
              <input matInput formControlName="accountName" placeholder="Enter account name">
              @if (form.get('accountName')?.hasError('required')) {
                <mat-error>Account name is required</mat-error>
              }
            </mat-form-field>

            @if (!isEditMode()) {
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Account Type</mat-label>
                <mat-select formControlName="accountType">
                  @for (type of accountTypes; track type) {
                    <mat-option [value]="type">{{ type }}</mat-option>
                  }
                </mat-select>
              </mat-form-field>

              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Owner User ID</mat-label>
                <input matInput formControlName="ownerUserId" placeholder="Enter owner user ID">
                @if (form.get('ownerUserId')?.hasError('required')) {
                  <mat-error>Owner user ID is required</mat-error>
                }
              </mat-form-field>
            }

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Subscription Tier</mat-label>
              <mat-select formControlName="subscriptionTier">
                <mat-option value="Free">Free</mat-option>
                <mat-option value="Basic">Basic</mat-option>
                <mat-option value="Professional">Professional</mat-option>
                <mat-option value="Enterprise">Enterprise</mat-option>
              </mat-select>
            </mat-form-field>

            <div class="form-actions">
              <button mat-button type="button" (click)="cancel()">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="isSaving() || form.invalid">
                @if (isSaving()) {
                  Saving...
                } @else {
                  {{ isEditMode() ? 'Update' : 'Create' }}
                }
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    }
  `,
  styles: [`
    .full-width {
      width: 100%;
    }

    mat-card {
      max-width: 600px;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 8px;
      margin-top: 16px;
    }
  `]
})
export class AccountFormComponent implements OnInit {
  private readonly accountService = inject(AccountService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);

  accountTypes = Object.values(AccountType);
  private accountId = '';

  form = this.fb.group({
    accountName: ['', Validators.required],
    accountType: [AccountType.Individual],
    ownerUserId: ['', Validators.required],
    subscriptionTier: ['Free', Validators.required]
  });

  ngOnInit(): void {
    this.accountId = this.route.snapshot.paramMap.get('id') || '';
    if (this.accountId && this.accountId !== 'new') {
      this.isEditMode.set(true);
      this.loadAccount();
    }
  }

  loadAccount(): void {
    this.isLoading.set(true);
    this.accountService.getById(this.accountId).subscribe({
      next: (account) => {
        this.form.patchValue({
          accountName: account.accountName,
          subscriptionTier: account.subscriptionTier
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.router.navigate(['/accounts']);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.isSaving.set(true);

    if (this.isEditMode()) {
      const { accountName, subscriptionTier } = this.form.value;
      this.accountService.update(this.accountId, {
        accountName: accountName!,
        subscriptionTier: subscriptionTier!
      }).subscribe({
        next: () => {
          this.router.navigate(['/accounts']);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      const { accountName, accountType, ownerUserId, subscriptionTier } = this.form.value;
      this.accountService.create({
        accountName: accountName!,
        accountType: accountType!,
        ownerUserId: ownerUserId!,
        subscriptionTier: subscriptionTier!
      }).subscribe({
        next: () => {
          this.router.navigate(['/accounts']);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/accounts']);
  }
}
