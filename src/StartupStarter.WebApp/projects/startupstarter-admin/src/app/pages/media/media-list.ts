import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { Router } from '@angular/router';
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
import { MediaService } from '../../services';
import { Media } from '../../models';

@Component({
  selector: 'app-media-list',
  standalone: true,
  imports: [
    FormsModule,
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
      title="Media Library"
      subtitle="Manage media files"
      icon="perm_media"
    >
      <button mat-raised-button color="primary" (click)="openUploadDialog()">
        <mat-icon>upload</mat-icon>
        Upload Media
      </button>
    </app-page-header>

    <mat-card class="media-list__card">
      <mat-card-content>
        <div class="media-list__toolbar">
          <mat-form-field appearance="outline" class="media-list__search">
            <mat-label>Search media</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by filename...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading media..."></app-loading-spinner>
        } @else if (filteredMedia().length === 0) {
          <app-empty-state
            icon="perm_media"
            title="No media found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by uploading your first media file'"
            [actionLabel]="searchQuery() ? undefined : 'Upload Media'"
            actionIcon="upload"
            (action)="openUploadDialog()"
          ></app-empty-state>
        } @else {
          <div class="media-list__grid">
            @for (media of filteredMedia(); track media.mediaId) {
              <div class="media-list__item">
                <div class="media-list__preview" [class.media-list__preview--image]="isImage(media)">
                  @if (isImage(media)) {
                    <img [src]="media.url" [alt]="media.fileName">
                  } @else {
                    <mat-icon class="media-list__file-icon">{{ getFileIcon(media) }}</mat-icon>
                  }
                </div>
                <div class="media-list__info">
                  <span class="media-list__name text-body-medium">{{ media.fileName }}</span>
                  <span class="media-list__size text-body-small">{{ formatSize(media.fileSize) }}</span>
                </div>
                <div class="media-list__actions">
                  <button mat-icon-button [matMenuTriggerFor]="menu">
                    <mat-icon>more_vert</mat-icon>
                  </button>
                  <mat-menu #menu="matMenu">
                    <button mat-menu-item (click)="copyUrl(media)">
                      <mat-icon>link</mat-icon>
                      <span>Copy URL</span>
                    </button>
                    <button mat-menu-item (click)="downloadMedia(media)">
                      <mat-icon>download</mat-icon>
                      <span>Download</span>
                    </button>
                    <button mat-menu-item (click)="deleteMedia(media)" class="media-list__delete">
                      <mat-icon>delete</mat-icon>
                      <span>Delete</span>
                    </button>
                  </mat-menu>
                </div>
              </div>
            }
          </div>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .media-list {
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

      &__grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
        gap: var(--spacing-md);
      }

      &__item {
        border: 1px solid var(--mat-sys-outline-variant);
        border-radius: var(--spacing-sm);
        overflow: hidden;
        background: var(--mat-sys-surface);
      }

      &__preview {
        height: 150px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: var(--mat-sys-surface-variant);

        &--image img {
          width: 100%;
          height: 100%;
          object-fit: cover;
        }
      }

      &__file-icon {
        font-size: 48px;
        width: 48px;
        height: 48px;
        color: var(--mat-sys-on-surface-variant);
      }

      &__info {
        padding: var(--spacing-sm);
        display: flex;
        flex-direction: column;
      }

      &__name {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      &__size {
        color: var(--mat-sys-on-surface-variant);
      }

      &__actions {
        padding: 0 var(--spacing-xs) var(--spacing-xs);
        display: flex;
        justify-content: flex-end;
      }

      &__delete {
        color: var(--mat-sys-error);
      }
    }
  `]
})
export class MediaList implements OnInit {
  readonly router = inject(Router);
  private readonly mediaService = inject(MediaService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly mediaItems = signal<Media[]>([]);
  readonly searchQuery = signal('');

  readonly filteredMedia = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.mediaItems();
    return this.mediaItems().filter(m =>
      m.fileName.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadMedia();
  }

  private loadMedia(): void {
    this.mediaService.getAll().subscribe({
      next: (media) => {
        this.mediaItems.set(media);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  isImage(media: Media): boolean {
    return media.mimeType.startsWith('image/');
  }

  getFileIcon(media: Media): string {
    if (media.mimeType.startsWith('video/')) return 'video_file';
    if (media.mimeType.startsWith('audio/')) return 'audio_file';
    if (media.mimeType.includes('pdf')) return 'picture_as_pdf';
    if (media.mimeType.includes('word') || media.mimeType.includes('document')) return 'description';
    if (media.mimeType.includes('excel') || media.mimeType.includes('spreadsheet')) return 'table_chart';
    return 'insert_drive_file';
  }

  formatSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
  }

  openUploadDialog(): void {
    this.snackBar.open('Upload dialog coming soon', 'Close', { duration: 3000 });
  }

  copyUrl(media: Media): void {
    navigator.clipboard.writeText(media.url);
    this.snackBar.open('URL copied to clipboard', 'Close', { duration: 3000 });
  }

  downloadMedia(media: Media): void {
    window.open(media.url, '_blank');
  }

  deleteMedia(media: Media): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete Media',
        message: `Are you sure you want to delete "${media.fileName}"?`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.mediaService.delete(media.mediaId).subscribe({
          next: () => {
            this.snackBar.open('Media deleted successfully', 'Close', { duration: 3000 });
            this.loadMedia();
          }
        });
      }
    });
  }
}
