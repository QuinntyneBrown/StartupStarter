import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../../components/shared';
import { UserService, RoleService } from '../../../services';
import { Role } from '../../../models';

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
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss'
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
