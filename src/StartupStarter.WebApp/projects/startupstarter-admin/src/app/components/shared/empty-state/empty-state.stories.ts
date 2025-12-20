import type { Meta, StoryObj } from '@storybook/angular';
import { EmptyStateComponent } from './empty-state.component';

const meta: Meta<EmptyStateComponent> = {
  title: 'Shared/EmptyState',
  component: EmptyStateComponent,
  parameters: {
    layout: 'centered'
  },
  tags: ['autodocs'],
  argTypes: {
    icon: {
      control: 'text',
      description: 'Material icon name to display'
    },
    title: {
      control: 'text',
      description: 'The main title text'
    },
    message: {
      control: 'text',
      description: 'Optional descriptive message'
    },
    actionLabel: {
      control: 'text',
      description: 'Label for the action button (if any)'
    },
    action: {
      action: 'clicked',
      description: 'Event emitted when action button is clicked'
    }
  }
};

export default meta;
type Story = StoryObj<EmptyStateComponent>;

export const Default: Story = {
  args: {
    icon: 'inbox',
    title: 'No items found',
    message: 'There are no items to display at this time.'
  }
};

export const WithAction: Story = {
  args: {
    icon: 'add_circle_outline',
    title: 'No content yet',
    message: 'Get started by creating your first piece of content.',
    actionLabel: 'Create Content'
  }
};

export const NoUsers: Story = {
  args: {
    icon: 'people_outline',
    title: 'No users found',
    message: 'Invite team members to get started with collaboration.',
    actionLabel: 'Invite Users'
  }
};

export const NoSearchResults: Story = {
  args: {
    icon: 'search_off',
    title: 'No results found',
    message: 'Try adjusting your search criteria or filters.',
    actionLabel: 'Clear Filters'
  }
};

export const NoMedia: Story = {
  args: {
    icon: 'perm_media',
    title: 'No media files',
    message: 'Upload images, videos, or documents to your media library.',
    actionLabel: 'Upload Files'
  }
};

export const NoWorkflows: Story = {
  args: {
    icon: 'account_tree',
    title: 'No workflows configured',
    message: 'Set up automated workflows to streamline your processes.',
    actionLabel: 'Create Workflow'
  }
};

export const NoApiKeys: Story = {
  args: {
    icon: 'vpn_key',
    title: 'No API keys',
    message: 'Generate API keys to integrate with external services.',
    actionLabel: 'Generate Key'
  }
};

export const ErrorState: Story = {
  args: {
    icon: 'error_outline',
    title: 'Something went wrong',
    message: 'We encountered an error while loading the data. Please try again.',
    actionLabel: 'Retry'
  }
};

export const NoNotifications: Story = {
  args: {
    icon: 'notifications_none',
    title: 'All caught up!',
    message: "You don't have any new notifications."
  }
};

export const WithoutIcon: Story = {
  args: {
    title: 'Nothing here yet',
    message: 'Content will appear here once available.',
    actionLabel: 'Learn More'
  }
};

export const TitleOnly: Story = {
  args: {
    title: 'No data available'
  }
};
