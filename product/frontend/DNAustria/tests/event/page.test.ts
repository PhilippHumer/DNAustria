import {test, expect} from "playwright/test";
import { firstValueFrom } from "rxjs";
import { createDemoEvent } from "../utils/even.utils";
import { login } from "../utils/create.util";

test.beforeEach(async ({page}) => {
  await login(page);
})

test('check if navigating to events URL displays event overview', async ({ page }) => {
    await page.goto('http://localhost:4200/events');

    await expect(page).toHaveURL(/events/);

    await expect(page.locator('app-events')).toBeVisible();
});

test('check if navigating to an event detail URL displays the detail page', async ({ page }) => {
  await page.goto('http://localhost:4200/events');
  var newEvent = await createDemoEvent();

  page.goto(`http://localhost:4200/event-details/${newEvent.id}`);
  await expect(page.locator("h1").first()).toHaveText(newEvent.name);
});

test('check if using the back button in the event detail navigates back to events overview', async ({page}) => {
  var newEvent = await createDemoEvent();
  page.goto(`http://localhost:4200/event-details/${newEvent.id}`);

  var backButton = page.locator(".btn-outline-secondary").first();
  await backButton.click();

  await expect(page).toHaveURL(`http://localhost:4200/events`)
});
