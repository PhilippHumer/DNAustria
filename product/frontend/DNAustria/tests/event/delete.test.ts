import { test, expect } from '@playwright/test';
import { createDemoEvent } from '../utils/even.utils';
import { login } from "../utils/create.util";

test.beforeEach(async ({page}) => {
  await login(page);
})

test('check if cancel delete does not remove element', async ({page}) => {
  // Go to your app
  await page.goto('http://localhost:4200/events');

  // --- 1. Click "Delete" on first event card ---

  var elemButtonRef = page.getByTestId('delete-event').first();
  await elemButtonRef.click();

  // --- 2. Check popup opens ---
  const popup = page.locator('.delete-confirm');

  await expect(popup).toBeVisible();

  // --- 3. Validate popup structure ---

  // Header

  await page.locator(".btn-outline-secondary").filter({hasText: "Cancel"}).click();

  await expect(popup).toBeHidden();

  await expect(elemButtonRef).toBeEnabled();
});

test('check if confirm delete does remove element', async ({page}) => {
  var event = await createDemoEvent("current-delete-item!");

  // Go to your app
  await page.goto('http://localhost:4200/events');

  // --- 1. Click "Delete" on first event card ---

  var elemButtonRef = page.locator("app-eventcard").filter({hasText: "current-delete-item!"}).getByTestId('delete-event').first();
  await elemButtonRef.click();

  // --- 2. Check popup opens ---
  const popup = page.locator('.delete-confirm');

  await expect(popup).toBeVisible();

  // --- 3. Validate popup structure ---

  // Header

  await page.locator(".btn-danger").filter({hasText: "Delete"}).click();

  await expect(popup).toBeHidden();

  await expect(page.locator("app-eventcard").filter({hasText: "current-delete-item!"})).toHaveCount(0);
});

