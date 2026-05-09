import { test, expect } from '@playwright/test';
import { login } from "../utils/create.util";

test.beforeEach(async ({page}) => {
  await login(page);
})
test('check if edit popup structure is correct', async ({page}) => {
  // Go to your app
  await page.goto('http://localhost:4200/events');

  // --- 1. Click "Edit" on first event card ---
  await page.getByLabel('Edit event').first().click();

  // --- 2. Check popup opens ---
  const popup = page.locator('.event-popup');

  await expect(popup).toBeVisible();

  // --- 3. Validate popup structure ---

  // Header
  await expect(popup.locator('.event-popup-header__title')).toHaveText('Edit Event');

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
})

test('check if empty input field sets edit button to disabled', async ({ page }) => {
  // Go to your app
  await page.goto('http://localhost:4200/events');

  // --- 1. Click "Edit" on first event card ---
  await page.getByLabel('Edit event').first().click();

  // --- 2. Check popup opens ---
  const popup = page.locator('.event-popup');
  const saveButton = popup.getByRole('button', { name: 'Save Changes' });

  // Clear required fields to simulate "no input"
  await popup.locator('#event-name').fill('');
  await popup.locator('#event-description').fill('');
  await popup.locator('#event-link').fill('');

  // You may also need to clear date fields depending on validation:
  await popup.locator('#event-start').fill('');
  await popup.locator('#event-end').fill('');

  // Assertion: button disabled
  await expect(saveButton).toBeDisabled();
});

test('check if submitting a valid event form edits the event', async ({ page }) => {
  await page.goto('http://localhost:4200/events');

  // 2️⃣ Click the first edit button on an event card
  const firstCard = page.locator('app-eventcard').first();
  const editButton = firstCard.locator('.btn-outline-secondary'); // class for edit button
  await editButton.click();

  // 3️⃣ Wait for the popup to appear
  const popup = page.locator('.event-popup');
  await expect(popup).toBeVisible();

  let randNr = 8;

  // 4️⃣ Fill out required fields
  await page.fill('#event-name', `My Test Event ${randNr}`);
  await page.selectOption('#event-classification', '0'); // Scheduled
  await page.fill('#event-description', 'This is a test description for the event.');
  await page.fill('#event-start', '2026-03-30T09:00');
  await page.fill('#event-end', '2026-03-30T12:00');
  await page.fill('#event-link', 'https://example.com');

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
  const saveButton = popup.locator('.event-action--primary');
  await expect(saveButton).toBeEnabled();

  // 6️⃣ Click the save button
  await saveButton.click();

  // 7️⃣ Optionally verify that popup closes or a success toast appears
  await expect(popup).toHaveCount(0); // popup disappears

  // Reload the page to check persistence
  await page.reload();

  const updatedCard = page
    .locator('app-eventcard')
    .filter({ hasText: `My Test Event ${randNr}` })
    .first();

  await expect(updatedCard).toBeVisible();

});

test('check if edit event cancel does not alter existing event', async ({page}) => {
    await page.goto('http://localhost:4200/events');

  // 2️⃣ Click the first edit button on an event card
  const firstCard = page.locator('app-eventcard').first();
  const editButton = firstCard.locator('.btn-outline-secondary'); // class for edit button
  await editButton.click();

  // 3️⃣ Wait for the popup to appear
  const popup = page.locator('.event-popup');
  await expect(popup).toBeVisible();

  let randNr = 8;

  // 4️⃣ Fill out required fields
let initTitle = await page.locator("#event-name").inputValue()

  await page.fill('#event-name', `My Test Event ${randNr}`);
  await page.selectOption('#event-classification', '0'); // Scheduled
  await page.fill('#event-description', 'This is a test description for the event.');
  await page.fill('#event-start', '2026-03-30T09:00');
  await page.fill('#event-end', '2026-03-30T12:00');
  await page.fill('#event-link', 'https://example.com');

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
  const cancelButton = popup.locator('.event-action--secondary');
  await expect(cancelButton).toBeEnabled();

  // 6️⃣ Click the save button
  await cancelButton.click();

  // 7️⃣ Optionally verify that popup closes or a success toast appears
  await expect(popup).toHaveCount(0); // popup disappears

  // Reload the page to check persistence
  await page.reload();

  // Find the first event card again
  const updatedCard = page.locator('app-eventcard').first();

  // Verify the event name was updated
  await expect(updatedCard.locator('a.h6')).toHaveText(initTitle!);

  var compVal = await firstCard.locator('.card-body p.small').first().textContent();
  // Optionally check other fields (description, program, status badges)
  await expect(updatedCard.locator('.card-body p.small').first()).toContainText(compVal!);
});
