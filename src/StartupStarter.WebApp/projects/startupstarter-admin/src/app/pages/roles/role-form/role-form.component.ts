import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../../components/shared';
import { RoleService } from '../../../services';
import { SYSTEM_PERMISSIONS } from '../../../models';

@Component({
  selector: 'app-role-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatExpansionModule,
    PageHeaderComponent,
    LoadingSpinnerComponent
  ],
  templateUrl: './role-form.component.html',
  styleUrl: './role-form.component.scss'
})
export class RoleFormComponent implements OnInit {
  private readonly roleService = inject(RoleService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly isEditMode = signal(false);
  private roleId = '';
  selectedPermissions = new Set<string>();

  form = this.fb.group({
    roleName: ['', Validators.required],
    description: ['']
  });

  permissionGroups = this.groupPermissions();

  ngOnInit(): void {
    this.roleId = this.route.snapshot.paramMap.get('id') || '';
    if (this.roleId && this.roleId !== 'new') {
      this.isEditMode.set(true);
      this.loadRole();
    }
  }

  loadRole(): void {
    this.isLoading.set(true);
    this.roleService.getById(this.roleId).subscribe({
      next: (role) => {
        this.form.patchValue({ roleName: role.roleName, description: role.description });
        this.selectedPermissions = new Set(role.permissions);
        this.isLoading.set(false);
      },
      error: () => { this.isLoading.set(false); this.router.navigate(['/roles']); }
    });
  }

  groupPermissions(): { name: string; permissions: string[] }[] {
    const groups: Record<string, string[]> = {};
    SYSTEM_PERMISSIONS.forEach(p => {
      const [resource] = p.split(':');
      const name = resource.charAt(0).toUpperCase() + resource.slice(1);
      if (!groups[name]) groups[name] = [];
      groups[name].push(p);
    });
    return Object.entries(groups).map(([name, permissions]) => ({ name, permissions }));
  }

  formatPermission(perm: string): string {
    const [, action] = perm.split(':');
    return action.charAt(0).toUpperCase() + action.slice(1);
  }

  getSelectedCount(permissions: string[]): number {
    return permissions.filter(p => this.selectedPermissions.has(p)).length;
  }

  togglePermission(perm: string): void {
    if (this.selectedPermissions.has(perm)) {
      this.selectedPermissions.delete(perm);
    } else {
      this.selectedPermissions.add(perm);
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSaving.set(true);
    const { roleName, description } = this.form.value;
    const permissions = Array.from(this.selectedPermissions);

    if (this.isEditMode()) {
      this.roleService.update(this.roleId, { roleName: roleName!, description: description!, permissions }).subscribe({
        next: () => this.router.navigate(['/roles']),
        error: () => this.isSaving.set(false)
      });
    } else {
      this.roleService.create({ roleName: roleName!, description: description!, permissions }).subscribe({
        next: () => this.router.navigate(['/roles']),
        error: () => this.isSaving.set(false)
      });
    }
  }

  cancel(): void { this.router.navigate(['/roles']); }
}
