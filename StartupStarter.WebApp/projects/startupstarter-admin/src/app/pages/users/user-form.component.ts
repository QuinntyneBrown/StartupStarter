import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../components/shared';
import { UserService, RoleService } from '../../services';
import { Role } from '../../models';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatChipsModule,
    PageHeaderComponent,
    LoadingSpinnerComponent
  ],
  template: `
    <app-page-header
      [title]="isEditMode() ? 'Edit User' : 'Invite User'"
      [subtitle]="isEditMode() ? 'Update user details' : 'Send an invitation to a new user'"
      icon="person">
    </app-page-header>

    @if (isLoading()) {
      <app-loading-spinner></app-loading-spinner>
    } @else {
      <mat-card>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Email</mat-label>
              <input matInput type="email" formControlName="email" placeholder="user@example.com">
              @if (form.get('email')?.hasError('required')) {
                <mat-error>Email is required</mat-error>
              }
              @if (form.get('email')?.hasError('email')) {
                <mat-error>Please enter a valid email</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>First Name</mat-label>
              <input matInput formControlName="firstName">
              @if (form.get('firstName')?.hasError('required')) {
                <mat-error>First name is required</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Last Name</mat-label>
              <input matInput formControlName="lastName">
              @if (form.get('lastName')?.hasError('required')) {
                <mat-error>Last name is required</mat-error>
              }
            </mat-form-field>

            @if (!isEditMode()) {
              <mat-form-field appearance="outline" class="full-width">
                <mat-label>Roles</mat-label>
                <mat-select formControlName="roleIds" multiple>
                  @for (role of roles(); track role.roleId) {
                    <mat-option [value]="role.roleId">{{ role.roleName }}</mat-option>
                  }
                </mat-select>
              </mat-form-field>
            }

            <div class="form-actions">
              <button mat-button type="button" (click)="cancel()">Cancel</button>
              <button mat-flat-button color="primary" type="submit" [disabled]="isSaving() || form.invalid">
                @if (isSaving()) {
                  Saving...
                } @else {
                  {{ isEditMode() ? 'Update' : 'Send Invitation' }}
                }
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    }
  `,
  styles: [`
    .full-width { width: 100%; }
    mat-card { max-width: 600px; }
    .form-actions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 16px; }
  `]
})
export class UserFormComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly roleService = inject(RoleService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);
  readonly roles = signal<Role[]>([]);
  private userId = '';

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    roleIds: [[] as string[]]
  });

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id') || '';
    if (this.userId && this.userId !== 'invite') {
      this.isEditMode.set(true);
      this.loadUser();
    }
    this.loadRoles();
  }

  loadUser(): void {
    this.isLoading.set(true);
    this.userService.getById(this.userId).subscribe({
      next: (user) => {
        this.form.patchValue({ email: user.email, firstName: user.firstName, lastName: user.lastName });
        this.isLoading.set(false);
      },
      error: () => { this.isLoading.set(false); this.router.navigate(['/users']); }
    });
  }

  loadRoles(): void {
    this.roleService.getAll().subscribe({ next: (roles) => this.roles.set(roles) });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSaving.set(true);

    if (this.isEditMode()) {
      const { firstName, lastName } = this.form.value;
      this.userService.update(this.userId, { firstName: firstName!, lastName: lastName! }).subscribe({
        next: () => this.router.navigate(['/users']),
        error: () => this.isSaving.set(false)
      });
    } else {
      const { email, firstName, lastName, roleIds } = this.form.value;
      this.userService.invite({ email: email!, firstName: firstName!, lastName: lastName!, roleIds: roleIds! }).subscribe({
        next: () => this.router.navigate(['/users']),
        error: () => this.isSaving.set(false)
      });
    }
  }

  cancel(): void { this.router.navigate(['/users']); }
}
