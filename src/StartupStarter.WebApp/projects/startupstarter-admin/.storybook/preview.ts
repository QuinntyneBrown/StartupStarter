import type { Preview } from '@storybook/angular';
import { applicationConfig } from '@storybook/angular';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';

const preview: Preview = {
  decorators: [
    applicationConfig({
      providers: [
        provideAnimationsAsync(),
        provideRouter([])
      ]
    })
  ],
  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
    backgrounds: {
      default: 'light',
      values: [
        { name: 'light', value: '#fafafa' },
        { name: 'dark', value: '#303030' },
        { name: 'white', value: '#ffffff' }
      ]
    }
  },
};

export default preview;