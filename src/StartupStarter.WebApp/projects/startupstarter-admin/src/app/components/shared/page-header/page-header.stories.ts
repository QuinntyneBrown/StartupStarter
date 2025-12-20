import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata } from '@storybook/angular';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderComponent } from './page-header.component';

const meta: Meta<PageHeaderComponent> = {
  title: 'Shared/PageHeader',
  component: PageHeaderComponent,
  decorators: [
    moduleMetadata({
      imports: [MatButtonModule, MatIconModule]
    })
  ],
  parameters: {
    layout: 'padded'
  },
  tags: ['autodocs'],
  argTypes: {
    title: {
      control: 'text',
      description: 'The main title of the page'
    },
    subtitle: {
      control: 'text',
      description: 'Optional subtitle text displayed below the title'
    },
    icon: {
      control: 'text',
      description: 'Material icon name to display'
    }
  }
};

export default meta;
type Story = StoryObj<PageHeaderComponent>;

export const Default: Story = {
  args: {
    title: 'Page Title',
    subtitle: 'This is a subtitle describing the page',
    icon: 'dashboard'
  }
};

export const WithoutSubtitle: Story = {
  args: {
    title: 'Simple Page Title',
    icon: 'settings'
  }
};

export const WithoutIcon: Story = {
  args: {
    title: 'Page Without Icon',
    subtitle: 'A clean header without an icon'
  }
};

export const TitleOnly: Story = {
  args: {
    title: 'Minimal Header'
  }
};

export const UserManagement: Story = {
  args: {
    title: 'User Management',
    subtitle: 'Manage user accounts and permissions',
    icon: 'people'
  }
};

export const ContentManagement: Story = {
  args: {
    title: 'Content Library',
    subtitle: 'Browse and manage your content',
    icon: 'article'
  }
};

export const WithActionButton: Story = {
  args: {
    title: 'Users',
    subtitle: 'Manage your users',
    icon: 'people'
  },
  render: (args) => ({
    props: args,
    template: `
      <app-page-header [title]="title" [subtitle]="subtitle" [icon]="icon">
        <button mat-flat-button color="primary">
          <mat-icon>add</mat-icon>
          Add User
        </button>
      </app-page-header>
    `
  })
};

export const WithMultipleActions: Story = {
  args: {
    title: 'Content Items',
    subtitle: 'Manage your content library',
    icon: 'article'
  },
  render: (args) => ({
    props: args,
    template: `
      <app-page-header [title]="title" [subtitle]="subtitle" [icon]="icon">
        <button mat-stroked-button>
          <mat-icon>filter_list</mat-icon>
          Filter
        </button>
        <button mat-flat-button color="primary">
          <mat-icon>add</mat-icon>
          Create
        </button>
      </app-page-header>
    `
  })
};
