import {test, expect} from "playwright/test";


test.describe('Dashboard', () => {
  test('check if navigating to dashboard URL displays dashboard', async ({ page }) => {
    await page.goto('http://localhost:4200/dashboard');

    await expect(page).toHaveURL(/dashboard/);

    await expect(page.locator('app-dashboard')).toBeVisible();
  });

  test('check if navigating to a not-assigned URL redirects to dashboard', async ({page}) => {
    await page.goto("http://localhost:4200/fdasjflsaf");

    await expect(page).toHaveURL(/dashboard/);
    await expect(page.locator('app-dashboard')).toBeVisible();
  })
});
