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
import { PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent, ConfirmDialogComponent, BadgeType } from '../../components/shared';
import { ContentService } from '../../services';
import { Content, ContentStatus } from '../../models';

@Component({
  selector: 'app-content-list',
  standalone: true,
  imports: [
    FormsModule, MatCardModule, MatTableModule, MatButtonModule, MatIconModule, MatMenuModule,
    MatFormFieldModule, MatInputModule, PageHeaderComponent, LoadingSpinnerComponent, EmptyStateComponent, StatusBadgeComponent
  ],
  template: `
    <app-page-header title="Content" subtitle="Manage your content" icon="article">
      <button mat-flat-button color="primary" (click)="create()">
        <mat-icon>add</mat-icon>
        New Content
      </button>
    </app-page-header>

    <mat-card>
      <mat-card-content>
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Search content</mat-label>
          <input matInput [(ngModel)]="searchQuery" placeholder="Search by title...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        @if (isLoading()) {
          <app-loading-spinner message="Loading content..."></app-loading-spinner>
        } @else if (filtered().length === 0) {
          <app-empty-state icon="article" title="No content found" actionLabel="Create Content" (action)="create()"></app-empty-state>
        } @else {
          <table mat-table [dataSource]="filtered()" class="full-width">
            <ng-container matColumnDef="title">
              <th mat-header-cell *matHeaderCellDef>Title</th>
              <td mat-cell *matCellDef="let content">{{ content.title }}</td>
            </ng-container>
            <ng-container matColumnDef="type">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let content">{{ content.contentType }}</td>
            </ng-container>
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let content">
                <app-status-badge [label]="content.status" [type]="getStatusType(content.status)"></app-status-badge>
              </td>
            </ng-container>
            <ng-container matColumnDef="author">
              <th mat-header-cell *matHeaderCellDef>Author</th>
              <td mat-cell *matCellDef="let content">{{ content.authorName || content.authorId }}</td>
            </ng-container>
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let content">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="edit(content)"><mat-icon>edit</mat-icon><span>Edit</span></button>
                  @if (content.status !== 'Published') {
                    <button mat-menu-item (click)="publish(content)"><mat-icon>publish</mat-icon><span>Publish</span></button>
                  } @else {
                    <button mat-menu-item (click)="unpublish(content)"><mat-icon>unpublished</mat-icon><span>Unpublish</span></button>
                  }
                  <button mat-menu-item (click)="delete(content)"><mat-icon color="warn">delete</mat-icon><span>Delete</span></button>
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
  styles: [`.search-field { width: 100%; max-width: 400px; margin-bottom: 16px; } .full-width { width: 100%; }`]
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
