import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, ConfirmDialogComponent } from '../../components/shared';
import { MediaService } from '../../services';
import { Media, MediaType } from '../../models';

@Component({
  selector: 'app-media-list',
  standalone: true,
  imports: [
    FormsModule, MatCardModule, MatButtonModule, MatIconModule, MatMenuModule,
    MatFormFieldModule, MatInputModule, PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent
  ],
  template: `
    <app-page-header title="Media" subtitle="Manage media files" icon="perm_media">
      <button mat-flat-button color="primary" (click)="fileInput.click()">
        <mat-icon>cloud_upload</mat-icon>
        Upload
      </button>
      <input #fileInput type="file" hidden (change)="onFileSelected($event)" multiple accept="image/*,video/*,.pdf">
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Search media</mat-label>
          <input matInput [(ngModel)]="searchQuery" placeholder="Search by filename...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        @if (isLoading()) {
          <app-loading-spinner message="Loading media..."></app-loading-spinner>
        } @else if (filtered().length === 0) {
          <app-empty-state icon="perm_media" title="No media found" message="Upload your first file" actionLabel="Upload" (action)="fileInput.click()"></app-empty-state>
        } @else {
          <div class="media-grid">
            @for (media of filtered(); track media.mediaId) {
              <mat-card class="media-card">
                <div class="media-preview" [class.image]="media.fileType === 'Image'">
                  @if (media.thumbnailUrl) {
                    <img [src]="media.thumbnailUrl" [alt]="media.fileName">
                  } @else {
                    <mat-icon>{{ getMediaIcon(media.fileType) }}</mat-icon>
                  }
                </div>
                <mat-card-content>
                  <p class="media-name">{{ media.fileName }}</p>
                  <p class="media-info">{{ formatFileSize(media.fileSize) }}</p>
                </mat-card-content>
                <mat-card-actions>
                  <button mat-icon-button (click)="download(media)"><mat-icon>download</mat-icon></button>
                  <button mat-icon-button [matMenuTriggerFor]="menu"><mat-icon>more_vert</mat-icon></button>
                  <mat-menu #menu="matMenu">
                    <button mat-menu-item (click)="delete(media)"><mat-icon color="warn">delete</mat-icon><span>Delete</span></button>
                  </mat-menu>
                </mat-card-actions>
              </mat-card>
            }
          </div>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .search-field { width: 100%; max-width: 400px; margin-bottom: 16px; }
    .media-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 16px; }
    .media-card { overflow: hidden; }
    .media-preview { height: 150px; display: flex; align-items: center; justify-content: center; background: #f5f5f5; }
    .media-preview.image img { width: 100%; height: 100%; object-fit: cover; }
    .media-preview mat-icon { font-size: 48px; width: 48px; height: 48px; color: rgba(0,0,0,0.38); }
    .media-name { font-size: 14px; margin: 0; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .media-info { font-size: 12px; color: rgba(0,0,0,0.6); margin: 4px 0 0; }
  `]
})
export class MediaListComponent implements OnInit {
  private readonly mediaService = inject(MediaService);
  private readonly dialog = inject(MatDialog);

  readonly isLoading = signal(true);
  readonly mediaList = signal<Media[]>([]);
  searchQuery = '';

  readonly filtered = computed(() => {
    const q = this.searchQuery.toLowerCase();
    return this.mediaList().filter(m => m.fileName.toLowerCase().includes(q));
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.mediaService.getAll().subscribe({
      next: (media) => { this.mediaList.set(media); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  getMediaIcon(type: MediaType): string {
    switch (type) {
      case MediaType.Image: return 'image';
      case MediaType.Video: return 'videocam';
      case MediaType.Document: return 'description';
      case MediaType.Audio: return 'audiotrack';
      default: return 'insert_drive_file';
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(1) + ' MB';
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      Array.from(input.files).forEach(file => {
        this.mediaService.upload({ file }).subscribe(() => this.load());
      });
    }
  }

  download(media: Media): void {
    this.mediaService.download(media.mediaId).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = media.fileName;
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  delete(media: Media): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete Media', message: `Delete "${media.fileName}"?`, confirmText: 'Delete', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) this.mediaService.delete(media.mediaId).subscribe(() => this.load());
    });
  }
}
