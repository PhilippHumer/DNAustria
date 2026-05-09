import { expect, Page } from 'playwright/test';
export async function login(page:Page){
      await page.goto("http://localhost:4200/login");
      await expect(page).toHaveURL(/login/);
      await page.getByLabel("Username").fill("developer");
      await page.getByLabel("Password").fill("developer")
      await page.getByRole('button', { name: 'Sign in with LDAP' }).click();


      await page.locator('app-dashboard').waitFor();
}
