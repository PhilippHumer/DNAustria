import {test, expect} from "playwright/test";
import { login } from "./utils/create.util";

test.beforeEach(async ({page}) => {
  await login(page);
})

test.afterEach(async ({page}) => {

})

test.describe('Organizations', () => {
  test('check if navigating to organizations URL displays organizations', async ({ page }) => {
    await page.goto('http://localhost:4200/organizations');

    await expect(page).toHaveURL(/organizations/);

    await expect(page.locator('app-organizations')).toBeVisible();
  });

  test('check if adding an organization opens popup dialog', async ({page}) => {
    await page.goto('http://localhost:4200/organizations');

    await expect(page).toHaveURL(/organizations/);

      // Wait for the button to be visible (optional, but good practice)
    await page.getByRole('button', { name: 'Add Organization' }).waitFor();

    // Click the button
    await page.getByRole('button', { name: 'Add Organization' }).click();

    // Optional: Assert something after the click, e.g., a new form appears
    await expect(page.getByText('Register a new partner organization')).toBeVisible();
    await expect(page.getByText('Address').first()).toBeVisible();
    await expect(page.getByText('Name').first()).toBeVisible();
    });

    test('check if adding an organization with valid elements works', async ({page}) => {
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

        await page.getByRole("button", {name: "Delete"}).last().click();
    })

    test('check if cancelling add organization with valid data does not add new element', async ({page}) => {
        await page.goto('http://localhost:4200/organizations');
        await page.getByRole('button', { name: 'Add Organization' }).click();

        await page.getByLabel("name").fill("");
        await page.getByLabel("street").fill("test-safsdfsf")
        await page.getByLabel("zip").fill("1234");
        await page.getByLabel("city").fill("test-safsafs");

        await expect(page.getByRole('button', { name: 'Create Organization' })).toBeDisabled();
    })

    test('check if adding an organization with invalid elements does not work', async ({page}) => {
        await page.goto('http://localhost:4200/organizations');
        await page.getByRole('button', { name: 'Add Organization' }).click();

        await page.getByLabel("name").fill("test-after-cancel");
        await page.getByLabel("street").fill("test-safsdfsf")
        await page.getByLabel("zip").fill("1234");
        await page.getByLabel("city").fill("test-safsafs");

        await page.getByRole('button', { name: 'Cancel' }).click();
        await page.locator('app-organizations').waitFor()

        await expect(page.getByText('test-after-cancel')).toBeHidden();
    })

    test('check if editing an organization with invalid elements does not work', async ({page}) => {
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

        await page.getByPlaceholder("Organization name").last().fill("");

        await expect(page.getByRole("button", {name: "Update Organization"})).toBeDisabled();

        await page.getByRole("button", {name: "Cancel"}).click();

        await page.getByRole('button', {name: "Delete organization"}).first().click();

        await expect(page.getByText("Confirm Delete").first()).toBeVisible()

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

        await page.getByRole("button", {name: "Delete"}).last().click();
    })
});
