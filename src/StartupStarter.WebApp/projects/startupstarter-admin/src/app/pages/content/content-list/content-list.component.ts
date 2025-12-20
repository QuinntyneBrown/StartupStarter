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
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent, BadgeType } from '../../../components/shared';
import { ContentService } from '../../../services';
import { Content, ContentStatus } from '../../../models';

@Component({
  selector: 'app-content-list',
  standalone: true,
  imports: [
    FormsModule, MatCardModule, MatTableModule, MatButtonModule, MatIconModule, MatMenuModule,
    MatFormFieldModule, MatInputModule, PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  templateUrl: './content-list.component.html',
  styleUrl: './content-list.component.scss'
})
export class ContentListComponent implements OnInit {
  private readonly contentService = inject(ContentService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  readonly isLoading = signal(true);
  readonly contents = signal<Content[]>([]);
  searchQuery = '';
  displayedColumns = ['title', 'type', 'status', 'author', 'actions'];

  readonly filtered = computed(() => {
    const q = this.searchQuery.toLowerCase();
    return this.contents().filter(c => c.title.toLowerCase().includes(q));
  });

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.contentService.getAll().subscribe({
      next: (contents) => { this.contents.set(contents); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  getStatusType(status: ContentStatus): BadgeType {
    switch (status) {
      case ContentStatus.Published: return 'success';
      case ContentStatus.Draft: return 'neutral';
      case ContentStatus.Review: return 'info';
      case ContentStatus.Approved: return 'success';
      case ContentStatus.Archived: return 'warning';
      default: return 'neutral';
    }
  }

  create(): void { this.router.navigate(['/content/new']); }
  edit(content: Content): void { this.router.navigate(['/content', content.contentId]); }
  publish(content: Content): void { this.contentService.publish(content.contentId).subscribe(() => this.load()); }
  unpublish(content: Content): void { this.contentService.unpublish(content.contentId, { reason: 'Manual unpublish' }).subscribe(() => this.load()); }

  delete(content: Content): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Delete Content', message: `Delete "${content.title}"?`, confirmText: 'Delete', confirmColor: 'warn' }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) this.contentService.delete(content.contentId).subscribe(() => this.load());
    });
  }
}
