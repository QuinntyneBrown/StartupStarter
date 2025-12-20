import { Component, inject, signal, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { PageHeaderComponent, LoadingSpinnerComponent } from '../../components/shared';
import { RoleService } from '../../services';
import { SYSTEM_PERMISSIONS } from '../../models';

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
  template: `
    <app-page-header
      [title]="isEditMode() ? 'Edit Role' : 'New Role'"
      [subtitle]="isEditMode() ? 'Update role permissions' : 'Create a new role with permissions'"
      icon="admin_panel_settings">
    </app-page-header>

    @if (isLoading()) {
      <app-loading-spinner></app-loading-spinner>
    } @else {
      <mat-card>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Role Name</mat-label>
              <input matInput formControlName="roleName">
              @if (form.get('roleName')?.hasError('required')) {
                <mat-error>Role name is required</mat-error>
              }
            </mat-form-field>

            <mat-form-field appearance="outline" class="full-width">
              <mat-label>Description</mat-label>
              <textarea matInput formControlName="description" rows="3"></textarea>
            </mat-form-field>

            <h3>Permissions</h3>
            <mat-accordion>
              @for (group of permissionGroups; track group.name) {
                <mat-expansion-panel>
                  <mat-expansion-panel-header>
                    <mat-panel-title>{{ group.name }}</mat-panel-title>
                    <mat-panel-description>{{ getSelectedCount(group.permissions) }} / {{ group.permissions.length }}</mat-panel-description>
                  </mat-expansion-panel-header>
                  <div class="permissions-grid">
                    @for (perm of group.permissions; track perm) {
                      <mat-checkbox [checked]="selectedPermissions.has(perm)" (change)="togglePermission(perm)">
                        {{ formatPermission(perm) }}
                      </mat-checkbox>
                    }
                  </div>
                </mat-expansion-panel>
              }
            </mat-accordion>

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
  styles: [`
    .full-width { width: 100%; }
    mat-card { max-width: 800px; }
    .form-actions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 24px; }
    .permissions-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 8px; padding: 16px 0; }
    h3 { margin: 24px 0 16px; }
  `]
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
