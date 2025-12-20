import type { StorybookConfig } from '@storybook/angular';

const config: StorybookConfig = {
  stories: [
    '../src/**/*.stories.@(js|jsx|mjs|ts|tsx)'
  ],
  framework: '@storybook/angular',
  core: {
    disableTelemetry: true
  }
};
export default config;