import type { StorybookConfig } from '@storybook/angular';
import { readFileSync } from 'fs';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const config: StorybookConfig = {
  stories: [
    '../src/**/*.stories.@(js|jsx|mjs|ts|tsx)'
  ],
  framework: '@storybook/angular',
  core: {
    disableTelemetry: true
  },
  previewHead: (head) => {
    const cssPath = join(__dirname, 'preview-styles.css');
    const css = readFileSync(cssPath, 'utf8');
    return `
      ${head}
      <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap" rel="stylesheet">
      <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
      <style>${css}</style>
    `;
  }
};
export default config;