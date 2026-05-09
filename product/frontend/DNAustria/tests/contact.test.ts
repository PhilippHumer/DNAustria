import {test, expect} from "playwright/test";
import { login } from "./utils/create.util";

test.beforeEach(async ({page}) => {
  await login(page);
})

test.describe('Contacts', () => {
  test('check if navigating to contacts URL displays contacts', async ({ page }) => {
    await page.goto('http://localhost:4200/contacts');

    await expect(page).toHaveURL(/contacts/);

    await expect(page.locator('app-contacts')).toBeVisible();
  });

  test('check if adding a contact opens popup dialog', async ({page}) => {
    await page.goto('http://localhost:4200/contacts');

    await expect(page).toHaveURL(/contacts/);

      // Wait for the button to be visible (optional, but good practice)
    await page.getByRole('button', { name: 'Add Contact' }).waitFor();

    // Click the button
    await page.getByRole('button', { name: 'Add Contact' }).click();

    // Optional: Assert something after the click, e.g., a new form appears
    await expect(page.getByText('Register a new event contact')).toBeVisible();
    await expect(page.getByText('Name').first()).toBeVisible();
    await expect(page.getByText('Email').first()).toBeVisible();
    await expect(page.getByText('Telefon').first()).toBeVisible();
    });

    test('check if adding a contact with valid elements works', async ({page}) => {
        await page.goto('http://localhost:4200/contacts');
        await page.getByRole('button', { name: 'Add Contact' }).click();

        await page.getByLabel("Name").fill("test-sdfasf");
        await page.getByLabel("Email").fill("test@mail.com")
        await page.getByLabel("Telefon").fill("+43123412341");

        await page.getByRole('button', { name: 'Create Contact' }).click();
        await page.locator('app-contacts').waitFor()

        await expect(page.getByText('test-sdfasf').first()).toBeVisible();

        await page.getByRole('button', {name: "Delete contact"}).first().click();

        await expect(page.getByText("Delete").first()).toBeVisible()

        await page.getByRole("button", {name: "Delete"}).last().click();
    })

    test('check if cancelling add contact with valid data does not add new element', async ({page}) => {
        await page.goto('http://localhost:4200/contacts');
        await page.getByRole('button', { name: 'Add Contact' }).click();

        await page.getByLabel("Name").fill("test-sdfasf");
        await page.getByLabel("Email").fill("test@mail.com")
        await page.getByLabel("Telefon").fill("+43123412341");

        await expect(page.getByRole('button', { name: 'Create Contact' })).toBeEnabled();
    })

    test('check if adding a contact with invalid elements does not work', async ({page}) => {
        await page.goto('http://localhost:4200/contacts');
        await page.getByRole('button', { name: 'Add Contact' }).click();

        await page.getByLabel("Name").fill("test-after-cancel");
        await page.getByLabel("Email").fill("invalid mail")
        await page.getByLabel("Telefon").fill("invalid phone");

        await page.getByRole('button', { name: 'Cancel' }).click();
        await page.locator('app-contacts').waitFor()

        await expect(page.getByText('test-after-cancel')).toBeHidden();
    })

    test('check if editing a contact with invalid elements does not work', async ({page}) => {
         await page.goto('http://localhost:4200/contacts');
        await page.getByRole('button', { name: 'Add Contact' }).click();
        await page.getByLabel("Name").fill("test-sdfasf");
        await page.getByLabel("Email").fill("test@mail.com")
        await page.getByLabel("Telefon").fill("+43123412341");

        await page.getByRole('button', { name: 'Create Contact' }).click();
        await page.locator('app-contacts').waitFor()

        await expect(page.getByText('test-sdfasf').first()).toBeVisible();

        await page.getByRole('button', {name: "Edit contact"}).first().click();

        await expect(page.getByText("Edit Contact").first()).toBeVisible()

        await page.getByPlaceholder("Full name").last().fill("");

        await expect(page.getByRole("button", {name: "Save Changes"})).toBeEnabled();

        await page.getByRole("button", {name: "Cancel"}).click();

        await page.getByRole('button', {name: "Delete"}).first().click();

        await expect(page.getByText("Delete").first()).toBeVisible()

        await page.getByRole("button", {name: "Delete"}).last().click();
    })

    test('check if editing an organization with valid elements does work', async ({page}) => {
         await page.goto('http://localhost:4200/organizations');
        await page.getByRole('button', { name: 'Add Organization' }).click();

        await page.getByLabel("name").fill("test-sdfasf");
        await page.getByLabel("street").fill("test-safsdfsf")
        await page.getByLabel("zip").fill("1234");
        await page.getByLabel("city").fill("test-safsafs");

        await page.getByRole('button', { name: 'Create Organization' }).click();
        await page.locator('app-organizations').waitFor()

        await expect(page.getByText('test-sdfasf')).toBeVisible();

        await page.getByRole('button', {name: "Edit organization"}).first().click();

        await expect(page.getByText("Update Organization").first()).toBeVisible()

        await page.getByPlaceholder("Organization name").last().fill("this is my updated organization!");

        await page.getByRole("button", {name: "Update Organization"}).click();

        await expect(page.getByText("this is my updated organization")).toBeVisible();

        await page.getByRole('button', {name: "Delete organization"}).first().click();

        await expect(page.getByText("Confirm Delete").first()).toBeVisible()

        await page.getByRole("button", {name: "Delete"}).last().click();
    })


    test('check if editing an organization and cancel does not alter the current object', async ({page}) => {
         await page.goto('http://localhost:4200/organizations');
        await page.getByRole('button', { name: 'Add Organization' }).click();

        await page.getByLabel("name").fill("test-sdfasf");
        await page.getByLabel("street").fill("test-safsdfsf")
        await page.getByLabel("zip").fill("1234");
        await page.getByLabel("city").fill("test-safsafs");

        await page.getByRole('button', { name: 'Create Organization' }).click();
        await page.locator('app-organizations').waitFor()

        await expect(page.getByText('test-sdfasf')).toBeVisible();

        await page.getByRole('button', {name: "Edit organization"}).first().click();

        await expect(page.getByText("Update Organization").first()).toBeVisible()

        await page.getByPlaceholder("Organization name").last().fill("this is my updated organization!");

        await page.getByRole("button", {name: "Cancel"}).click();

        await expect(page.getByText("this is my updated organization")).toBeHidden();

        await page.getByRole('button', {name: "Delete organization"}).first().click();

        await expect(page.getByText("Confirm Delete").first()).toBeVisible()

        await page.getByRole("button", {name: "Delete"}).last().click();
    })

    test('check if cancel during delete an organization works', async ({page}) => {
         await page.goto('http://localhost:4200/organizations');
        await page.getByRole('button', { name: 'Add Organization' }).click();

        await page.getByLabel("name").fill("test-sdfasf");
        await page.getByLabel("street").fill("test-safsdfsf")
        await page.getByLabel("zip").fill("1234");
        await page.getByLabel("city").fill("test-safsafs");

        await page.getByRole('button', { name: 'Create Organization' }).click();
        await page.locator('app-organizations').waitFor()

        await expect(page.getByText('test-sdfasf')).toBeVisible();

        await page.getByRole('button', {name: "Delete organization"}).first().click();

        await expect(page.getByText("Confirm Delete").first()).toBeVisible()

        await page.getByRole("button", {name: "Cancel"}).click();

        await expect(page.getByText("test-sdfasf")).toBeVisible();

        await page.getByRole('button', {name: "Delete organization"}).first().click();

        await expect(page.getByText("Confirm Delete").first()).toBeVisible()

        await page.getByRole("button", {name: "Delete"}).last().click();
    })

    test('check if confirm deletion deletes object', async ({page}) => {
         await page.goto('http://localhost:4200/contacts');
        await page.getByRole('button', { name: 'Add Contact' }).click();

        await page.getByLabel("Name").fill("test-sdfasf");
        await page.getByLabel("Email").fill("test@mail.com")
        await page.getByLabel("Telefon").fill("+43123412341");

        await page.getByRole('button', { name: 'Create Contact' }).click();
        await page.locator('app-contacts').waitFor()

        await expect(page.getByText('test-sdfasf')).toBeVisible();

        await page.getByRole('button', {name: "Delete Contact"}).first().click();

        await expect(page.getByText("Delete").first()).toBeVisible()

        await page.getByRole("button", {name: "Delete"}).last().click();

        await expect(page.getByText("test-sdfasf")).toBeHidden();
    })
});
