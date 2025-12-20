import { Component, inject, signal, OnInit, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormsModule } from '@angular/forms';
import { PageHeader } from '../../components/shared/page-header';
import { StatusBadge } from '../../components/shared/status-badge';
import { LoadingSpinner } from '../../components/shared/loading-spinner';
import { EmptyState } from '../../components/shared/empty-state';
import { ConfirmDialog } from '../../components/shared/confirm-dialog';
import { ContentService } from '../../services';
import { Content, ContentStatus } from '../../models';

@Component({
  selector: 'app-content-list',
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
    PageHeader,
    StatusBadge,
    LoadingSpinner,
    EmptyState
  ],
  template: `
    <app-page-header
      title="Content"
      subtitle="Manage content items"
      icon="article"
    >
      <button mat-raised-button color="primary" routerLink="/content/new">
        <mat-icon>add</mat-icon>
        New Content
      </button>
    </app-page-header>

    <mat-card class="content-list__card">
      <mat-card-content>
        <div class="content-list__toolbar">
          <mat-form-field appearance="outline" class="content-list__search">
            <mat-label>Search content</mat-label>
            <input matInput [(ngModel)]="searchQuery" placeholder="Search by title...">
            <mat-icon matPrefix>search</mat-icon>
          </mat-form-field>
        </div>

        @if (isLoading()) {
          <app-loading-spinner message="Loading content..."></app-loading-spinner>
        } @else if (filteredContent().length === 0) {
          <app-empty-state
            icon="article"
            title="No content found"
            [message]="searchQuery() ? 'Try adjusting your search criteria' : 'Get started by creating your first content'"
            [actionLabel]="searchQuery() ? undefined : 'Create Content'"
            actionIcon="add"
            (action)="router.navigate(['/content/new'])"
          ></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filteredContent()" class="content-list__table">
            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Title</th>
              <td mat-cell *matCellDef="let content">
                <a [routerLink]="['/content', content.contentId]" class="content-list__link">
                  {{ content.title }}
                </a>
              </td>
            </ng-container>

            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let content">{{ content.contentType }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let content">
                <app-status-badge
                  [status]="getStatusType(content.status)"
                  [label]="content.status"
                ></app-status-badge>
              </td>
            </ng-container>

            <ng-container matColumnDef="version">
              <th mat-header-cell *matHeaderCellDef>Version</th>
              <td mat-cell *matCellDef="let content">v{{ content.currentVersion }}</td>
            </ng-container>

            <ng-container matColumnDef="updated">
              <th mat-header-cell *matHeaderCellDef>Updated</th>
              <td mat-cell *matCellDef="let content">{{ formatDate(content.updatedAt || content.createdAt) }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let content">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item [routerLink]="['/content', content.contentId]">
                    <mat-icon>visibility</mat-icon>
                    <span>View</span>
                  </button>
                  <button mat-menu-item [routerLink]="['/content', content.contentId, 'edit']">
                    <mat-icon>edit</mat-icon>
                    <span>Edit</span>
                  </button>
                  @if (content.status !== 'Published') {
                    <button mat-menu-item (click)="publishContent(content)">
                      <mat-icon>publish</mat-icon>
                      <span>Publish</span>
                    </button>
                  } @else {
                    <button mat-menu-item (click)="unpublishContent(content)">
                      <mat-icon>unpublished</mat-icon>
                      <span>Unpublish</span>
                    </button>
                  }
                  <button mat-menu-item (click)="deleteContent(content)" class="content-list__delete">
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
    .content-list {
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

      &__delete {
        color: var(--mat-sys-error);
      }
    }
  `]
})
export class ContentList implements OnInit {
  readonly router = inject(Router);
  private readonly contentService = inject(ContentService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  readonly isLoading = signal(true);
  readonly contents = signal<Content[]>([]);
  readonly searchQuery = signal('');

  readonly displayedColumns = ['title', 'type', 'status', 'version', 'updated', 'actions'];

  readonly filteredContent = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.contents();
    return this.contents().filter(c =>
      c.title.toLowerCase().includes(query)
    );
  });

  ngOnInit(): void {
    this.loadContent();
  }

  private loadContent(): void {
    this.contentService.getAll().subscribe({
      next: (contents) => {
        this.contents.set(contents);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  getStatusType(status: ContentStatus): 'success' | 'warning' | 'error' | 'info' | 'neutral' {
    switch (status) {
      case ContentStatus.Published: return 'success';
      case ContentStatus.Draft: return 'neutral';
      case ContentStatus.Review: return 'info';
      case ContentStatus.Approved: return 'success';
      case ContentStatus.Unpublished: return 'warning';
      case ContentStatus.Archived: return 'neutral';
      case ContentStatus.Deleted: return 'error';
      default: return 'neutral';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }

  publishContent(content: Content): void {
    this.contentService.publish(content.contentId).subscribe({
      next: () => {
        this.snackBar.open('Content published successfully', 'Close', { duration: 3000 });
        this.loadContent();
      }
    });
  }

  unpublishContent(content: Content): void {
    this.contentService.unpublish(content.contentId, 'Unpublished by admin').subscribe({
      next: () => {
        this.snackBar.open('Content unpublished successfully', 'Close', { duration: 3000 });
        this.loadContent();
      }
    });
  }

  deleteContent(content: Content): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: 'Delete Content',
        message: `Are you sure you want to delete "${content.title}"?`,
        confirmText: 'Delete',
        confirmColor: 'warn',
        icon: 'delete'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.contentService.delete(content.contentId).subscribe({
          next: () => {
            this.snackBar.open('Content deleted successfully', 'Close', { duration: 3000 });
            this.loadContent();
          }
        });
      }
    });
  }
}
