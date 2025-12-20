import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent } from '../../components/shared';
import { ProfileService } from '../../services';
import { Profile } from '../../models';

@Component({
  selector: 'app-profile-list',
  standalone: true,
  imports: [
    FormsModule, MatCardModule, MatTableModule, MatButtonModule, MatIconModule, MatMenuModule,
    MatFormFieldModule, MatInputModule, PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  template: `
    <app-page-header title="Profiles" subtitle="Manage user profiles" icon="person">
      <button mat-flat-button color="primary" (click)="create()">
        <mat-icon>add</mat-icon>
        New Profile
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Search profiles</mat-label>
          <input matInput [(ngModel)]="searchQuery" placeholder="Search...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        @if (isLoading()) {
          <app-loading-spinner message="Loading profiles..."></app-loading-spinner>
        } @else if (filtered().length === 0) {
          <app-empty-state icon="person" title="No profiles found" actionLabel="Create Profile" (action)="create()"></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filtered()" class="full-width">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let profile">
                {{ profile.profileName }}
                @if (profile.isDefault) {
                  <app-status-badge label="Default" type="info"></app-status-badge>
                }
              </td>
            </ng-container>
            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let profile">{{ profile.profileType }}</td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let profile">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="edit(profile)"><mat-icon>edit</mat-icon><span>Edit</span></button>
                  <button mat-menu-item (click)="setDefault(profile)" [disabled]="profile.isDefault"><mat-icon>star</mat-icon><span>Set as Default</span></button>
                  <button mat-menu-item (click)="delete(profile)"><mat-icon color="warn">delete</mat-icon><span>Delete</span></button>
                </mat-menu>
              </td>
            </ng-container>
            <tr mat-header-row *matHeaderRowDef="['name', 'type', 'actions']"></tr>
            <tr mat-row *matRowDef="let row; columns: ['name', 'type', 'actions'];"></tr>
          </table>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`.search-field { width: 100%; max-width: 400px; margin-bottom: 16px; } .full-width { width: 100%; }`]
})
export class ProfileListComponent implements OnInit {
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly isLoading = signal(true);
  readonly profiles = signal<Profile[]>([]);
  searchQuery = '';

  readonly filtered = computed(() => {
    const q = this.searchQuery.toLowerCase();
    return this.profiles().filter(p => p.profileName.toLowerCase().includes(q));
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.profileService.getAll().subscribe({
      next: (profiles) => { this.profiles.set(profiles); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  create(): void { this.router.navigate(['/profiles/new']); }
  edit(profile: Profile): void { this.router.navigate(['/profiles', profile.profileId]); }
  setDefault(profile: Profile): void { this.profileService.setAsDefault(profile.profileId).subscribe(() => this.load()); }

  delete(profile: Profile): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete Profile', message: `Delete "${profile.profileName}"?`, confirmText: 'Delete', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) this.profileService.delete(profile.profileId).subscribe(() => this.load());
    });
  }
}
