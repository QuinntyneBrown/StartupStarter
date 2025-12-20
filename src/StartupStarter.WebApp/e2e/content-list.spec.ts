import { test, expect } from '@playwright/test';
import * as path from 'path';

test.describe('Content List Page', () => {
  test('capture mockup screenshot', async ({ page }) => {
    const mockupPath = '/home/user/StartupStarter/docs/features/content/mockups/content-list-mockup.html';
    await page.goto(`file://${mockupPath}`);
    await page.waitForLoadState('networkidle');
    await page.screenshot({
      path: 'e2e/screenshots/content-list-mockup.png',
      fullPage: true
    });
  });

  test('capture rendered page screenshot', async ({ page }) => {
    // Mock content data
    const mockContents = [
      { contentId: '1', title: 'Welcome to StartupStarter', contentType: 'Article', status: 'Published', authorName: 'John Doe', authorId: 'user-1' },
      { contentId: '2', title: 'Getting Started Guide', contentType: 'Tutorial', status: 'Draft', authorName: 'Jane Smith', authorId: 'user-2' },
      { contentId: '3', title: 'API Documentation', contentType: 'Documentation', status: 'Review', authorName: 'Tech Writer', authorId: 'user-3' },
      { contentId: '4', title: 'Product Roadmap 2025', contentType: 'Article', status: 'Published', authorName: 'Product Manager', authorId: 'user-4' },
      { contentId: '5', title: 'User Onboarding Tips', contentType: 'Blog', status: 'Draft', authorName: 'Marketing Team', authorId: 'user-5' }
    ];

    // Intercept API calls and return mock data
    await page.route('**/api/content', route => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockContents)
      });
    });

    // First go to the app to set localStorage
    await page.goto('http://localhost:4200/login');

    // Set up mock authentication in localStorage
    await page.evaluate(() => {
      const mockUser = {
        userId: 'test-user-id',
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        roles: ['admin'],
        permissions: ['*:*'],
        mfaEnabled: false
      };
      localStorage.setItem('auth_token', 'test-token');
      localStorage.setItem('refresh_token', 'test-refresh-token');
      localStorage.setItem('auth_user', JSON.stringify(mockUser));
    });

    // Now navigate to the content page
    await page.goto('http://localhost:4200/content');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(2000); // Wait for Angular to fully render
    await page.screenshot({
      path: 'e2e/screenshots/content-list-rendered.png',
      fullPage: true
    });
  });
});
