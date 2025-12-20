import type { Meta, StoryObj } from '@storybook/angular';
import { StatusBadgeComponent, BadgeType } from './status-badge.component';

const meta: Meta<StatusBadgeComponent> = {
  title: 'Shared/StatusBadge',
  component: StatusBadgeComponent,
  parameters: {
    layout: 'centered'
  },
  tags: ['autodocs'],
  argTypes: {
    label: {
      control: 'text',
      description: 'The text displayed in the badge'
    },
    type: {
      control: 'select',
      options: ['success', 'warning', 'error', 'info', 'neutral'] as BadgeType[],
      description: 'The type/color of the badge'
    }
  }
};

export default meta;
type Story = StoryObj<StatusBadgeComponent>;

export const Success: Story = {
  args: {
    label: 'Active',
    type: 'success'
  }
};

export const Warning: Story = {
  args: {
    label: 'Pending',
    type: 'warning'
  }
};

export const Error: Story = {
  args: {
    label: 'Inactive',
    type: 'error'
  }
};

export const Info: Story = {
  args: {
    label: 'In Progress',
    type: 'info'
  }
};

export const Neutral: Story = {
  args: {
    label: 'Draft',
    type: 'neutral'
  }
};

export const Published: Story = {
  args: {
    label: 'Published',
    type: 'success'
  }
};

export const Archived: Story = {
  args: {
    label: 'Archived',
    type: 'neutral'
  }
};

export const Rejected: Story = {
  args: {
    label: 'Rejected',
    type: 'error'
  }
};

export const UnderReview: Story = {
  args: {
    label: 'Under Review',
    type: 'info'
  }
};

export const AwaitingApproval: Story = {
  args: {
    label: 'Awaiting Approval',
    type: 'warning'
  }
};

export const AllTypes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 8px; flex-wrap: wrap;">
        <app-status-badge label="Success" type="success"></app-status-badge>
        <app-status-badge label="Warning" type="warning"></app-status-badge>
        <app-status-badge label="Error" type="error"></app-status-badge>
        <app-status-badge label="Info" type="info"></app-status-badge>
        <app-status-badge label="Neutral" type="neutral"></app-status-badge>
      </div>
    `
  })
};
