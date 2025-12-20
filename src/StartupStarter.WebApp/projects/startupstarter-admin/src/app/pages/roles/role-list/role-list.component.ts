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
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, ConfirmDialogComponent } from '../../../components/shared';
import { RoleService } from '../../../services';
import { Role } from '../../../models';

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
  templateUrl: './role-list.component.html',
  styleUrl: './role-list.component.scss'
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
