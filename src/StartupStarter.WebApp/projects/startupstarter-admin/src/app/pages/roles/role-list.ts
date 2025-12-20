import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { PageHeader } from '../../components/shared/page-header';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { EmptyState } from '../../components/shared/empty-state';
import { ConfirmDialog } from '../../components/shared/confirm-dialog';
import { RoleService } from '../../services';
import { Role } from '../../models';

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [
    RouterLink,
    FormsModule,
    MatTableModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatChipsModule,
    PageHeader,
    LoadingSpinner,
    EmptyState
  ],
  template: `
    <app-page-header
      title="Roles"
      subtitle="Manage roles and permissions"
      icon="admin_panel_settings"
    >
      <button mat-raised-button color="primary" routerLink="/roles/new">
        <mat-icon>add</mat-icon>
        New Role
      </button>
    </app-page-header>

    <mat-card class="role-list__card">
      <mat-card-content>
        <div class="role-list__toolbar">
          <mat-form-field appearance="outline" class="role-list__search">
            <mat-label>Search roles</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by name...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading roles..."></app-loading-spinner>
        } @else if (filteredRoles().length === 0) {
          <app-empty-state
            icon="admin_panel_settings"
            title="No roles found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by creating your first role'"
            [actionLabel]="searchQuery() ? undefined : 'Create Role'"
            actionIcon="add"
            (action)="router.navigate(['/roles/new'])"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredRoles()" class="role-list__table">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Role Name</th>
              <td mat-cell *matCellDef="let role">
                <a [routerLink]="['/roles', role.roleId]" class="role-list__link">
                  {{ role.roleName }}
                </a>
              </td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>Description</th>
              <td mat-cell *matCellDef="let role">{{ role.description }}</td>
            </ng-container>

            <ng-container matColumnDef="permissions">
              <th mat-header-cell *matHeaderCellDef>Permissions</th>
              <td mat-cell *matCellDef="let role">
                <div class="role-list__permissions">
                  @for (permission of role.permissions.slice(0, 3); track permission) {
                    <mat-chip>{{ permission }}</mat-chip>
                  }
                  @if (role.permissions.length > 3) {
                    <mat-chip>+{{ role.permissions.length - 3 }} more</mat-chip>
                  }
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="created">
              <th mat-header-cell *matHeaderCellDef>Created</th>
              <td mat-cell *matCellDef="let role">{{ formatDate(role.createdAt) }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let role">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/roles', role.roleId]">
                    <mat-icon>visibility</mat-icon>
                    <span>View Details</span>
                  </button>
                  <button mat-menu-item [routerLink]="['/roles', role.roleId, 'edit']">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  <button mat-menu-item (click)="deleteRole(role)" class="role-list__delete">
                    <mat-icon>delete</mat-icon>
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
    .role-list {
      &__card {
        overflow: hidden;
      }

      &__toolbar {
        display: flex;
        gap: var(--spacing-md);
        margin-bottom: var(--spacing-md);
      }

      &__search {
        flex: 1;
        max-width: 400px;
      }

      &__table {
        width: 100%;
      }

      &__link {
        color: var(--mat-sys-primary);
        text-decoration: none;
        font-weight: 500;

        &:hover {
          text-decoration: underline;
        }
      }

      &__permissions {
        display: flex;
        flex-wrap: wrap;
        gap: var(--spacing-xs);
      }

      &__delete {
        color: var(--mat-sys-error);
      }
    }
  `]
})
export class RoleList implements OnInit {
  readonly router = inject(Router);
  private readonly roleService = inject(RoleService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly roles = signal<Role[]>([]);
  readonly searchQuery = signal('');

  readonly displayedColumns = ['name', 'description', 'permissions', 'created', 'actions'];

  readonly filteredRoles = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.roles();
    return this.roles().filter(r =>
      r.roleName.toLowerCase().includes(query) ||
      r.description.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadRoles();
  }

  private loadRoles(): void {
    this.roleService.getAll().subscribe({
      next: (roles) => {
        this.roles.set(roles);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }

  deleteRole(role: Role): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete Role',
        message: `Are you sure you want to delete "${role.roleName}"? Users with this role will lose associated permissions.`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.roleService.delete(role.roleId).subscribe({
          next: () => {
            this.snackBar.open('Role deleted successfully', 'Close', { duration: 3000 });
            this.loadRoles();
          }
        });
      }
    });
  }
}
