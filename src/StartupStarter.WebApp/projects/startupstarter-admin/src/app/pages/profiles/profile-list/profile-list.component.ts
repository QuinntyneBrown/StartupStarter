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
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent } from '../../../components/shared';
import { ProfileService } from '../../../services';
import { Profile } from '../../../models';

@Component({
  selector: 'app-profile-list',
  standalone: true,
  imports: [
    FormsModule, MatCardModule, MatTableModule, MatButtonModule, MatIconModule, MatMenuModule,
    MatFormFieldModule, MatInputModule, PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  templateUrl: './profile-list.component.html',
  styleUrl: './profile-list.component.scss'
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
