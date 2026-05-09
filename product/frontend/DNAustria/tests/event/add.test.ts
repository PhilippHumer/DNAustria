import { test, expect } from '@playwright/test';
import { login } from "../utils/create.util";

test.beforeEach(async ({page}) => {
  await login(page);
})

test('check if add form can be opened', async ({page}) => {
  // Go to your app
  await page.goto('http://localhost:4200/events');

  // --- 1. Click "Edit" on first event card ---
  await page.getByTestId('add-event').click();

  // --- 2. Check popup opens ---
  const popup = page.locator('.event-popup');

  await expect(popup).toBeVisible();

  // --- 3. Validate popup structure ---

  // Header
  await expect(popup.locator('.event-popup-header__title')).toHaveText('Create Event');

  // Sections
  await expect(popup.getByText('Basics')).toBeVisible();
  await expect(popup.getByText('Schedule & Access')).toBeVisible();
  await expect(popup.getByText('Partners & Place')).toBeVisible();
  await expect(popup.getByText('Audience').first()).toBeVisible();

  // Key inputs (by id = very stable 👍)
  await expect(popup.locator('#event-name')).toBeVisible();
  await expect(popup.locator('#event-description')).toBeVisible();
  await expect(popup.locator('#event-start')).toBeVisible();
  await expect(popup.locator('#event-end')).toBeVisible();
  await expect(popup.locator('#event-link')).toBeVisible();

  // Check checkboxes exist
  await expect(popup.locator('#event-online')).toBeVisible();
  await expect(popup.locator('#event-fees')).toBeVisible();
});


test('check if submitting a valid event form creates a new event', async ({ page }) => {
  await page.goto('http://localhost:4200/events');

  // 2️⃣ Click the first edit button on an event card
  const addButton = page.getByTestId('add-event'); // class for edit button
  await addButton.click();

  // 3️⃣ Wait for the popup to appear
  const popup = page.locator('.event-popup');
  await expect(popup).toBeVisible();

  let randNr = 8;
  let eventTitle = `My new awesome test event ${randNr}`;

  // 4️⃣ Fill out required fields
  await page.fill('#event-name', eventTitle);
  await page.selectOption('#event-classification', '0'); // Scheduled
  await page.fill('#event-description', 'This is a test description for the event.');
  await page.fill('#event-start', '2026-03-30T09:00');
  await page.fill('#event-format', 'demo-format');
  await page.fill('#event-end', '2026-03-30T12:00');
  await page.fill('#event-link', 'https://example.com');
  await page.fill('#event-program', 'test');
  await page.fill('#event-program', 'test');
  // Optional: fill program or other fields
  await page.fill('#event-program', 'Test Program');

  // Check at least one target audience (required)
  const firstAudience = page.locator('#event-target-audiences input[type="checkbox"]').first();
  await firstAudience.check();

  // Check at least one topic (required)
  const firstTopic = page.locator('#event-topics input[type="checkbox"]').first();
  await firstTopic.check();

  // Select an organization (required)
  await page.selectOption('#event-organization', '1'); // adjust the value to a real option

  // 5️⃣ Verify save button is now enabled
  const saveButton = popup.getByText('Create Event').last();
  await expect(saveButton).toBeEnabled();

  // 6️⃣ Click the save button
  await saveButton.click();

  // 7️⃣ Optionally verify that popup closes or a success toast appears
  await expect(popup).toHaveCount(0); // popup disappears

  // Find the first event card again
  console.log(page.locator('app-eventcard'))
  const addedCard = page.locator('app-eventcard').last();

  // Verify the event name was updated
  await expect(addedCard.locator('a.h6')).toHaveText(eventTitle);

  // Optionally check other fields (description, program, status badges)
  await expect(addedCard.locator('.card-body p.small').first()).toContainText('This is a test description');
});
