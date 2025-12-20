import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, ConfirmDialogComponent } from '../../components/shared';
import { RoleService } from '../../services';
import { Role } from '../../models';

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [
    FormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatChipsModule,
    PageHeaderComponent,
    LoadingSpinnerComponent,
    EmptyStateComponent
  ],
  template: `
    <app-page-header title="Roles" subtitle="Manage roles and permissions" icon="admin_panel_settings">
      <button mat-flat-button color="primary" (click)="create()">
        <mat-icon>add</mat-icon>
        New Role
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Search roles</mat-label>
          <input matInput [(ngModel)]="searchQuery" placeholder="Search by name...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        @if (isLoading()) {
          <app-loading-spinner message="Loading roles..."></app-loading-spinner>
        } @else if (filtered().length === 0) {
          <app-empty-state
            icon="admin_panel_settings"
            title="No roles found"
            message="Create roles to manage user permissions"
            actionLabel="Create Role"
            (action)="create()">
          </app-empty-state>
        } @else {
          <table mat-table [dataSource]="filtered()" class="full-width">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Role Name</th>
              <td mat-cell *matCellDef="let role">
                {{ role.roleName }}
                @if (role.isSystemRole) {
                  <mat-chip class="system-chip">System</mat-chip>
                }
              </td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>Description</th>
              <td mat-cell *matCellDef="let role">{{ role.description }}</td>
            </ng-container>

            <ng-container matColumnDef="permissions">
              <th mat-header-cell *matHeaderCellDef>Permissions</th>
              <td mat-cell *matCellDef="let role">{{ role.permissions.length }} permissions</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let role">
                <button mat-icon-button [matMenuTriggerFor]="menu" [disabled]="role.isSystemRole">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="edit(role)">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  <button mat-menu-item (click)="delete(role)">
                    <mat-icon color="warn">delete</mat-icon>
                    <span>Delete</span>
                  </button>
                </mat-menu>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .search-field { width: 100%; max-width: 400px; margin-bottom: 16px; }
    .full-width { width: 100%; }
    .system-chip { font-size: 10px; margin-left: 8px; }
  `]
})
export class RoleListComponent implements OnInit {
  private readonly roleService = inject(RoleService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly isLoading = signal(true);
  readonly roles = signal<Role[]>([]);
  searchQuery = '';
  displayedColumns = ['name', 'description', 'permissions', 'actions'];

  readonly filtered = computed(() => {
    const query = this.searchQuery.toLowerCase();
    return this.roles().filter(r => r.roleName.toLowerCase().includes(query));
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.roleService.getAll().subscribe({
      next: (roles) => { this.roles.set(roles); this.isLoading.set(false); },
      error: () => { this.isLoading.set(false); }
    });
  }

  create(): void { this.router.navigate(['/roles/new']); }
  edit(role: Role): void { this.router.navigate(['/roles', role.roleId]); }

  delete(role: Role): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete Role', message: `Delete "${role.roleName}"? Users with this role will lose these permissions.`, confirmText: 'Delete', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.roleService.delete(role.roleId).subscribe(() => this.load());
      }
    });
  }
}
