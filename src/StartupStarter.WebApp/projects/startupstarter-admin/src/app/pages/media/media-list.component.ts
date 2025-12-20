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
  templateUrl: './media-list.component.html',
  styleUrl: './media-list.component.scss'
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
