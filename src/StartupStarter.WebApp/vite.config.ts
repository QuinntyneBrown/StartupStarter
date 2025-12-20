/// <reference types="vitest" />
import { defineConfig } from 'vite';
import angular from '@analogjs/vite-plugin-angular';

export default defineConfig({
  plugins: [
    angular({
      tsconfig: 'projects/startupstarter-admin/tsconfig.spec.json'
    })
  ],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['projects/startupstarter-admin/src/test-setup.ts'],
    include: ['projects/startupstarter-admin/src/**/*.spec.ts']
  }
});
