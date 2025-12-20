import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../../components/shared';
import { AccountService } from '../../../services';
import { AccountType } from '../../../models';

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
  templateUrl: './account-form.component.html',
  styleUrl: './account-form.component.scss'
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
