import type { Meta, StoryObj } from '@storybook/angular';
import { LoadingSpinnerComponent } from './loading-spinner.component';

const meta: Meta<LoadingSpinnerComponent> = {
  title: 'Shared/LoadingSpinner',
  component: LoadingSpinnerComponent,
  parameters: {
    layout: 'centered'
  },
  tags: ['autodocs'],
  argTypes: {
    diameter: {
      control: { type: 'range', min: 20, max: 100, step: 4 },
      description: 'The diameter of the spinner in pixels'
    },
    message: {
      control: 'text',
      description: 'Optional message displayed below the spinner'
    }
  }
};

export default meta;
type Story = StoryObj<LoadingSpinnerComponent>;

export const Default: Story = {
  args: {
    diameter: 48
  }
};

export const WithMessage: Story = {
  args: {
    diameter: 48,
    message: 'Loading data...'
  }
};

export const Small: Story = {
  args: {
    diameter: 24,
    message: 'Please wait...'
  }
};

export const Large: Story = {
  args: {
    diameter: 80,
    message: 'Loading dashboard...'
  }
};

export const LoadingUsers: Story = {
  args: {
    diameter: 48,
    message: 'Loading users...'
  }
};

export const LoadingContent: Story = {
  args: {
    diameter: 48,
    message: 'Fetching content...'
  }
};

export const SavingChanges: Story = {
  args: {
    diameter: 36,
    message: 'Saving changes...'
  }
};

export const NoMessage: Story = {
  args: {
    diameter: 48
  }
};

export const SpinnerSizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 48px; align-items: center;">
        <div style="text-align: center;">
          <app-loading-spinner [diameter]="24"></app-loading-spinner>
          <p style="margin-top: 8px; color: #666;">Small (24px)</p>
        </div>
        <div style="text-align: center;">
          <app-loading-spinner [diameter]="48"></app-loading-spinner>
          <p style="margin-top: 8px; color: #666;">Default (48px)</p>
        </div>
        <div style="text-align: center;">
          <app-loading-spinner [diameter]="72"></app-loading-spinner>
          <p style="margin-top: 8px; color: #666;">Large (72px)</p>
        </div>
      </div>
    `
  })
};
