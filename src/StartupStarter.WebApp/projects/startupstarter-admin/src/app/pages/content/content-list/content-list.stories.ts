import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ContentListComponent } from './content-list.component';
import { ContentService } from '../../../services';
import { ContentStatus } from '../../../models';

const mockContents = [
  { contentId: '1', title: 'Getting Started Guide', contentType: 'Article', status: ContentStatus.Published, authorName: 'John Doe' },
  { contentId: '2', title: 'Product Announcement', contentType: 'Blog Post', status: ContentStatus.Draft, authorName: 'Jane Smith' },
  { contentId: '3', title: 'API Documentation', contentType: 'Documentation', status: ContentStatus.Review, authorName: 'Bob Wilson' },
  { contentId: '4', title: 'User Tutorial', contentType: 'Tutorial', status: ContentStatus.Approved, authorName: 'Alice Brown' },
  { contentId: '5', title: 'Old Release Notes', contentType: 'Article', status: ContentStatus.Archived, authorName: 'Charlie Davis' }
];

const createMockContentService = (contents: typeof mockContents = mockContents, loadingDelay = 500) => ({
  getAll: () => of(contents).pipe(delay(loadingDelay)),
  publish: () => of(undefined),
  unpublish: () => of(undefined),
  delete: () => of(undefined)
});

const meta: Meta<ContentListComponent> = {
  title: 'Pages/Content/ContentList',
  component: ContentListComponent,
  decorators: [
    applicationConfig({
      providers: [
        provideRouter([
          { path: 'content/new', component: ContentListComponent },
          { path: 'content/:id', component: ContentListComponent }
        ])
      ]
    })
  ],
  parameters: {
    layout: 'padded'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<ContentListComponent>;

export const Default: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: ContentService, useValue: createMockContentService() }
      ]
    })
  ]
};

export const Loading: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: ContentService, useValue: createMockContentService(mockContents, 10000) }
      ]
    })
  ]
};

export const EmptyState: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        { provide: ContentService, useValue: createMockContentService([]) }
      ]
    })
  ]
};

export const AllPublished: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: ContentService,
          useValue: createMockContentService([
            { contentId: '1', title: 'Guide One', contentType: 'Article', status: ContentStatus.Published, authorName: 'John' },
            { contentId: '2', title: 'Guide Two', contentType: 'Article', status: ContentStatus.Published, authorName: 'Jane' },
            { contentId: '3', title: 'Guide Three', contentType: 'Article', status: ContentStatus.Published, authorName: 'Bob' }
          ])
        }
      ]
    })
  ]
};

export const AllDrafts: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: ContentService,
          useValue: createMockContentService([
            { contentId: '1', title: 'Draft Article', contentType: 'Article', status: ContentStatus.Draft, authorName: 'John' },
            { contentId: '2', title: 'Draft Blog', contentType: 'Blog Post', status: ContentStatus.Draft, authorName: 'Jane' }
          ])
        }
      ]
    })
  ]
};
