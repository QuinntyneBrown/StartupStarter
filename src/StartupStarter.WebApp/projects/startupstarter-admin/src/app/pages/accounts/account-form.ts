import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PageHeader } from '../../components/shared/page-header';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { AccountService } from '../../services';
import { Account, AccountType } from '../../models';

@Component({
  selector: 'app-account-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    PageHeader,
    LoadingSpinner
  ],
  template: `
    <app-page-header
      [title]="isEditMode() ? 'Edit Account' : 'New Account'"
      [subtitle]="isEditMode() ? 'Update account information' : 'Create a new organization account'"
      icon="business"
    >
      <button mat-button routerLink="/accounts">
        <mat-icon>arrow_back</mat-icon>
        Back to Accounts
      </button>
    </app-page-header>

    @if (isLoading()) {
      <app-loading-spinner message="Loading account..."></app-loading-spinner>
    } @else {
      <mat-card class="account-form__card">
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()" class="account-form__form">
            <mat-form-field appearance="outline" class="account-form__field">
              <mat-label>Account Name</mat-label>
              <input matInput formControlName="accountName" placeholder="Enter account name">
              @if (form.get('accountName')?.hasError('required')) {
                <mat-error>Account name is required</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="account-form__field">
              <mat-label>Account Type</mat-label>
              <mat-select formControlName="accountType">
                @for (type of accountTypes; track type) {
                  <mat-option [value]="type">{{ type }}</mat-option>
                }
              </mat-select>
              @if (form.get('accountType')?.hasError('required')) {
                <mat-error>Account type is required</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="account-form__field">
              <mat-label>Subscription Tier</mat-label>
              <mat-select formControlName="subscriptionTier">
                @for (tier of subscriptionTiers; track tier) {
                  <mat-option [value]="tier">{{ tier }}</mat-option>
                }
              </mat-select>
              @if (form.get('subscriptionTier')?.hasError('required')) {
                <mat-error>Subscription tier is required</mat-error>
              }
            </mat-form-field>

            @if (!isEditMode()) {
              <mat-form-field appearance="outline" class="account-form__field">
                <mat-label>Owner User ID</mat-label>
                <input matInput formControlName="ownerUserId" placeholder="Enter owner user ID">
                @if (form.get('ownerUserId')?.hasError('required')) {
                  <mat-error>Owner user ID is required</mat-error>
                }
              </mat-form-field>
            }

            <div class="account-form__actions">
              <button mat-button type="button" routerLink="/accounts">Cancel</button>
              <button mat-raised-button color="primary" type="submit" [disabled]="isSaving()">
                @if (isSaving()) {
                  Saving...
                } @else {
                  {{ isEditMode() ? 'Update Account' : 'Create Account' }}
                }
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    }
  `,
  styles: [`
    .account-form {
      &__card {
        max-width: 600px;
      }

      &__form {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-md);
      }

      &__field {
        width: 100%;
      }

      &__actions {
        display: flex;
        justify-content: flex-end;
        gap: var(--spacing-md);
        margin-top: var(--spacing-md);
      }
    }
  `]
})
export class AccountForm implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly accountService = inject(AccountService);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);

  readonly accountTypes = Object.values(AccountType);
  readonly subscriptionTiers = ['Free', 'Basic', 'Professional', 'Enterprise'];

  private accountId: string | null = null;

  form: FormGroup = this.fb.group({
    accountName: ['', [Validators.required]],
    accountType: [AccountType.Team, [Validators.required]],
    subscriptionTier: ['Professional', [Validators.required]],
    ownerUserId: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.accountId = this.route.snapshot.paramMap.get('id');
    if (this.accountId && this.accountId !== 'new') {
      this.isEditMode.set(true);
      this.loadAccount();
    }
  }

  private loadAccount(): void {
    if (!this.accountId) return;

    this.isLoading.set(true);
    this.accountService.getById(this.accountId).subscribe({
      next: (account) => {
        this.form.patchValue({
          accountName: account.accountName,
          accountType: account.accountType,
          subscriptionTier: account.subscriptionTier
        });
        this.form.get('ownerUserId')?.disable();
        this.isLoading.set(false);
      },
      error: () => {
        this.snackBar.open('Failed to load account', 'Close', { duration: 3000 });
        this.router.navigate(['/accounts']);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);

    if (this.isEditMode()) {
      this.accountService.update(this.accountId!, {
        accountName: this.form.value.accountName
      }).subscribe({
        next: () => {
          this.snackBar.open('Account updated successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/accounts', this.accountId]);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      this.accountService.create(this.form.value).subscribe({
        next: (account) => {
          this.snackBar.open('Account created successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/accounts', account.accountId]);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }
}
