import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata } from '@storybook/angular';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { ConfirmDialogComponent, ConfirmDialogData } from './confirm-dialog.component';

const mockDialogRef = {
  close: (result?: boolean) => console.log('Dialog closed with result:', result)
};

const meta: Meta<ConfirmDialogComponent> = {
  title: 'Shared/ConfirmDialog',
  component: ConfirmDialogComponent,
  decorators: [
    moduleMetadata({
      imports: [MatDialogModule, MatButtonModule],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef }
      ]
    })
  ],
  parameters: {
    layout: 'centered'
  },
  tags: ['autodocs']
};

export default meta;
type Story = StoryObj<ConfirmDialogComponent>;

export const DeleteConfirmation: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            title: 'Delete Item',
            message: 'Are you sure you want to delete this item? This action cannot be undone.',
            confirmText: 'Delete',
            cancelText: 'Cancel',
            confirmColor: 'warn'
          } as ConfirmDialogData
        }
      ]
    })
  ]
};

export const SaveConfirmation: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            title: 'Save Changes',
            message: 'Do you want to save your changes before leaving?',
            confirmText: 'Save',
            cancelText: 'Discard',
            confirmColor: 'primary'
          } as ConfirmDialogData
        }
      ]
    })
  ]
};

export const PublishConfirmation: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            title: 'Publish Content',
            message: 'This will make the content visible to all users. Are you sure you want to publish?',
            confirmText: 'Publish',
            cancelText: 'Not Yet',
            confirmColor: 'primary'
          } as ConfirmDialogData
        }
      ]
    })
  ]
};

export const DeactivateUser: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            title: 'Deactivate User',
            message: 'The user will no longer be able to access the system. Their data will be preserved.',
            confirmText: 'Deactivate',
            cancelText: 'Cancel',
            confirmColor: 'warn'
          } as ConfirmDialogData
        }
      ]
    })
  ]
};

export const RevokeApiKey: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            title: 'Revoke API Key',
            message: 'This will immediately invalidate the API key. Any applications using this key will lose access.',
            confirmText: 'Revoke Key',
            cancelText: 'Keep Active',
            confirmColor: 'warn'
          } as ConfirmDialogData
        }
      ]
    })
  ]
};

export const SimpleConfirmation: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            title: 'Confirm Action',
            message: 'Are you sure you want to proceed?'
          } as ConfirmDialogData
        }
      ]
    })
  ]
};

export const LogoutConfirmation: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            title: 'Sign Out',
            message: 'You will be signed out of your account. Any unsaved changes will be lost.',
            confirmText: 'Sign Out',
            cancelText: 'Stay Signed In',
            confirmColor: 'primary'
          } as ConfirmDialogData
        }
      ]
    })
  ]
};

export const ArchiveContent: Story = {
  decorators: [
    moduleMetadata({
      providers: [
        {
          provide: MAT_DIALOG_DATA,
          useValue: {
            title: 'Archive Content',
            message: 'This content will be moved to the archive. You can restore it later if needed.',
            confirmText: 'Archive',
            cancelText: 'Cancel',
            confirmColor: 'accent'
          } as ConfirmDialogData
        }
      ]
    })
  ]
};
