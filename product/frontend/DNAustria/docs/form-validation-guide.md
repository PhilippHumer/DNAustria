# Form Validation Guide (AI & Developer Reference)

This document is the **single source of truth** for building validated reactive forms in this Angular project. Follow every step exactly. When in doubt, refer to the contact form reference implementation listed at the end.

---

## Architecture Overview

Every validated form consists of exactly these parts:

```
shared/
  validate-input/
    validate-input.ts          # Reusable wrapper component (DO NOT MODIFY)

your-feature/
  your-form/
    your-form-group-factory.ts # Static factory: builds FormGroup + all validators
    your-form.ts               # Standalone form component (logic + ValidationMessages)
    your-form.html             # Template with <app-validate-input> wrappers
    your-form.css              # Styles (can be empty)
```

**Responsibilities:**

| Part | Does | Does NOT |
| ---- | ---- | -------- |
| **FormGroupFactory** | Creates `FormGroup`, attaches all validators (per-field + cross-field) | Know about UI, services, or Angular components |
| **Form Component (.ts)** | Injects services, defines `ValidationMessage[]` arrays, handles submit/cancel, loads data (e.g. dropdowns) | Contain validation logic (that belongs in the factory) |
| **Template (.html)** | Wraps inputs in `<app-validate-input>`, shows cross-field errors, handles submit button state | Contain logic beyond template expressions |
| **ValidateInput** | Shows/hides errors per `showOnDirty` flag | Need modification (shared, reusable) |
| **Parent Component** | Passes data via inputs, reacts to outputs (`formSubmit`, `formCancel`) | Contain form logic or validation |

---

## ValidateInput Component (shared, read-only reference)

Located at `src/app/shared/validate-input/validate-input.ts`. **Do not modify this file.** Use it as-is.

```ts
// src/app/shared/validate-input/validate-input.ts

import { Component, input } from '@angular/core';
import { AbstractControl } from '@angular/forms';

export interface ValidationMessage {
  key: string;
  message: string;
  /** Show this error as soon as the user starts typing (dirty), not only after blur (touched). */
  showOnDirty?: boolean;
}

@Component({
  selector: 'app-validate-input',
  template: `
    <ng-content />
    @for (vm of messages(); track vm.key) {
      @if (control()?.hasError(vm.key) && isVisible(vm)) {
        <div class="validation-error">{{ vm.message }}</div>
      }
    }
  `,
  styles: `
    :host { display: block; }
    .validation-error {
      color: #dc3545;
      font-size: 0.8rem;
      margin-top: 0.25rem;
    }
  `,
})
export class ValidateInput {
  readonly control = input.required<AbstractControl | null>();
  readonly messages = input<ValidationMessage[]>([]);

  protected isVisible(vm: ValidationMessage): boolean {
    const ctrl = this.control();
    if (!ctrl) return false;
    return vm.showOnDirty ? ctrl.dirty : ctrl.touched;
  }
}
```

### How `showOnDirty` controls error visibility

| `showOnDirty` | Error appears when... | Use for |
| --- | --- | --- |
| `false` (default) | User leaves the field (`touched`) | `required` -- do not show before the user even starts typing |
| `true` | User starts typing (`dirty`) | `minlength`, `maxlength`, `email`, `pattern` -- give instant live feedback while typing |

**Rule:** Always set `showOnDirty: true` for length, format, and pattern validators. Only omit it (or set `false`) for `required`.

---

## Step-by-Step: Create a New Form

### Step 1: Create the FormGroupFactory

Create a file `your-form-group-factory.ts` with a static `create(fb: FormBuilder): FormGroup` method.

**Rules:**

- Always use `fb.nonNullable.group(...)` so controls never hold `null`.
- Attach per-field validators as the second element of the control tuple: `['defaultValue', [Validator1, Validator2]]`.
- Attach cross-field (group-level) validators via the second argument: `{ validators: [...] }`.
- Cross-field validators receive the entire group as `AbstractControl` and read children via `control.get('fieldName')`.
- Custom validators that are specific to this form live as static methods on the factory class.
- Reusable validators shared across forms go in separate files under `shared/validators/`.

**Template:**

```ts
// src/app/your-feature/your-form/your-form-group-factory.ts

import { FormBuilder, FormGroup, AbstractControl, ValidationErrors, Validators } from '@angular/forms';

export class YourFormGroupFactory {
  static create(fb: FormBuilder): FormGroup {
    return fb.nonNullable.group(
      {
        fieldA: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
        fieldB: ['', [Validators.email]],
        fieldC: ['', [Validators.minLength(5), Validators.maxLength(20), Validators.pattern(/^\+?[0-9]*$/)]],
        fieldD: [''],
      },
      {
        validators: [YourFormGroupFactory.crossFieldValidator],
      },
    );
  }

  static crossFieldValidator(control: AbstractControl): ValidationErrors | null {
    const a = control.get('fieldB')?.value?.trim() ?? '';
    const b = control.get('fieldC')?.value?.trim() ?? '';
    if (!a && !b) {
      return { atLeastOneRequired: true };
    }
    return null;
  }
}
```

### Step 2: Create the Form Component (.ts)

**Rules:**

- The component is standalone (no `NgModule`).
- Import `ReactiveFormsModule` and `ValidateInput` in the `imports` array.
- Inject `FormBuilder` and any services the form needs (e.g. for dropdown data).
- Create the form via `YourFormGroupFactory.create(this.fb)` in the constructor.
- Define a `ValidationMessage[]` array for **each field that has validators**.
- Load dropdown/select data in the constructor (not lazily) using a signal + loading signal.
- Use `input()` for data passed from the parent (e.g. `editItem`, `submitting`, `submitError`, `isEditMode`).
- Use `output()` for events emitted to the parent (e.g. `formSubmit`, `formCancel`).
- If the form supports editing, use an `effect()` to populate the form when `editItem` changes.
- On submit: if form is invalid call `this.form.markAllAsTouched()` and return. Otherwise emit `formSubmit` with trimmed values.

**Template:**

```ts
// src/app/your-feature/your-form/your-form.ts

import { Component, effect, inject, input, output, signal } from '@angular/core';
import { FormGroup, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { SomeService } from '../../api/api/some.service';
import { YourFormGroupFactory } from './your-form-group-factory';
import { ValidateInput, ValidationMessage } from '../../shared/validate-input/validate-input';

export interface YourFormValue {
  fieldA: string;
  fieldB: string;
  fieldC: string;
  fieldD: string;
}

@Component({
  selector: 'app-your-form',
  imports: [ReactiveFormsModule, ValidateInput],
  templateUrl: './your-form.html',
  styleUrl: './your-form.css',
})
export class YourForm {
  private readonly fb = inject(FormBuilder);
  private readonly someService = inject(SomeService);

  // Inputs from parent
  readonly editItem = input<SomeDto | null>(null);
  readonly submitting = input(false);
  readonly submitError = input<string | null>(null);
  readonly isEditMode = input(false);

  // Outputs to parent
  readonly formSubmit = output<YourFormValue>();
  readonly formCancel = output<void>();

  // Dropdown data loaded from service
  readonly dropdownOptions = signal<string[]>([]);
  readonly dropdownLoading = signal(false);

  readonly form: FormGroup;

  // -- Validation messages per field --
  // IMPORTANT: showOnDirty: true for format/length validators (live feedback while typing)
  // IMPORTANT: showOnDirty omitted/false for 'required' (only show after blur)

  readonly fieldAMessages: ValidationMessage[] = [
    { key: 'required', message: 'Field A is required.' },
    { key: 'minlength', message: 'Field A must be at least 2 characters.', showOnDirty: true },
    { key: 'maxlength', message: 'Field A must not exceed 50 characters.', showOnDirty: true },
  ];

  readonly fieldBMessages: ValidationMessage[] = [
    { key: 'email', message: 'Please enter a valid email address.', showOnDirty: true },
  ];

  readonly fieldCMessages: ValidationMessage[] = [
    { key: 'minlength', message: 'Field C must be at least 5 characters.', showOnDirty: true },
    { key: 'maxlength', message: 'Field C must not exceed 20 characters.', showOnDirty: true },
    { key: 'pattern', message: 'Field C may only contain digits and an optional leading +.', showOnDirty: true },
  ];

  constructor() {
    this.form = YourFormGroupFactory.create(this.fb);
    this.loadDropdownOptions();

    // Populate form when editing an existing item
    effect(() => {
      const item = this.editItem();
      if (item) {
        this.form.setValue({
          fieldA: item.fieldA ?? '',
          fieldB: item.fieldB ?? '',
          fieldC: item.fieldC ?? '',
          fieldD: item.fieldD ?? '',
        });
        return;
      }
      this.form.reset({ fieldA: '', fieldB: '', fieldC: '', fieldD: '' });
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.formSubmit.emit({
      fieldA: value.fieldA.trim(),
      fieldB: value.fieldB.trim(),
      fieldC: value.fieldC.trim(),
      fieldD: value.fieldD.trim(),
    });
  }

  protected cancel(): void {
    this.formCancel.emit();
  }

  // Load dropdown data from API on component init
  private loadDropdownOptions(): void {
    this.dropdownLoading.set(true);
    this.someService.apiSomeGet().subscribe({
      next: (result) => {
        if (!Array.isArray(result)) {
          this.dropdownOptions.set([]);
          return;
        }
        const names = result
          .map((item: { name?: unknown }) => item.name)
          .filter((name: unknown): name is string => typeof name === 'string' && name.length > 0);
        this.dropdownOptions.set(names);
        this.dropdownLoading.set(false);
      },
      error: () => {
        this.dropdownOptions.set([]);
        this.dropdownLoading.set(false);
      },
    });
  }

  protected hasGroupError(errorKey: string): boolean {
    return this.form.hasError(errorKey) && (
      this.form.get('fieldB')!.touched || this.form.get('fieldC')!.touched
    );
  }
}
```

### Step 3: Create the Template (.html)

**Rules:**

- Wrap every validated input in `<app-validate-input>` with `[control]` and `[messages]`.
- Add `[class.is-invalid]="form.controls['fieldName'].invalid && form.controls['fieldName'].touched"` to inputs for Bootstrap red-border styling.
- Required fields get a `<span class="text-danger">*</span>` in the label.
- Cross-field errors are shown with `@if (hasGroupError('errorKey'))` outside of `<app-validate-input>`.
- Dropdowns show a "Loading..." option while data is loading, then "No selection" + the actual options.
- The form uses `(ngSubmit)="submit()"`, the cancel button is `type="button"`.
- The submit button is `[disabled]="submitting()"` and shows "Saving..." while submitting.

**Template:**

```html
<!-- src/app/your-feature/your-form/your-form.html -->

<form class="d-flex flex-column gap-3" [formGroup]="form" (ngSubmit)="submit()">

  <!-- Text input with validation -->
  <app-validate-input [control]="form.controls['fieldA']" [messages]="fieldAMessages">
    <label for="field-a" class="form-label fw-semibold">
      Field A <span class="text-danger">*</span>
    </label>
    <input
      id="field-a"
      type="text"
      class="form-control"
      formControlName="fieldA"
      [class.is-invalid]="form.controls['fieldA'].invalid && form.controls['fieldA'].touched"
      placeholder="Enter field A"
    />
  </app-validate-input>

  <!-- Email input -->
  <app-validate-input [control]="form.controls['fieldB']" [messages]="fieldBMessages">
    <label for="field-b" class="form-label fw-semibold">Email</label>
    <input
      id="field-b"
      type="email"
      class="form-control"
      formControlName="fieldB"
      [class.is-invalid]="form.controls['fieldB'].invalid && form.controls['fieldB'].touched"
      placeholder="email&#64;example.com"
    />
  </app-validate-input>

  <!-- Pattern-restricted input -->
  <app-validate-input [control]="form.controls['fieldC']" [messages]="fieldCMessages">
    <label for="field-c" class="form-label fw-semibold">Phone</label>
    <input
      id="field-c"
      type="tel"
      class="form-control"
      formControlName="fieldC"
      [class.is-invalid]="form.controls['fieldC'].invalid && form.controls['fieldC'].touched"
      placeholder="+43 ..."
    />
  </app-validate-input>

  <!-- Cross-field error (group-level validator) -->
  @if (hasGroupError('atLeastOneRequired')) {
    <div class="validation-error">Either email or phone must be provided.</div>
  }

  <!-- Dropdown loaded from API -->
  <div>
    <label for="field-d" class="form-label fw-semibold">Category</label>
    <select id="field-d" class="form-select" formControlName="fieldD">
      @if (dropdownLoading()) {
        <option value="">Loading...</option>
      } @else {
        <option value="">No selection</option>
        @for (opt of dropdownOptions(); track opt) {
          <option [value]="opt">{{ opt }}</option>
        }
      }
    </select>
  </div>

  <!-- Submit error -->
  @if (submitError()) {
    <div class="alert alert-danger py-2 mb-0">{{ submitError() }}</div>
  }

  <!-- Buttons -->
  <div class="d-flex justify-content-end gap-2 pt-2">
    <button type="button" class="btn btn-outline-secondary" (click)="cancel()">Cancel</button>
    <button type="submit" class="btn btn-primary" [disabled]="submitting()">
      @if (submitting()) {
        Saving...
      } @else {
        {{ isEditMode() ? 'Save Changes' : 'Create' }}
      }
    </button>
  </div>
</form>
```

### Step 4: Create the Parent Component

The parent (popup, page, dialog) **does not contain any form or validation logic**. It only:
- Imports the form component
- Passes data via inputs
- Handles `formSubmit` and `formCancel` outputs
- Calls the API to create/update the entity

```ts
// src/app/your-feature/your-popup/your-popup.ts

import { Component, computed, inject, input, output, signal } from '@angular/core';
import { YourApiService } from '../../api/api/your-api.service';
import { YourForm, YourFormValue } from '../your-form/your-form';

@Component({
  selector: 'app-your-popup',
  imports: [YourForm],
  templateUrl: './your-popup.html',
  styleUrl: './your-popup.css',
})
export class YourPopup {
  private readonly apiService = inject(YourApiService);

  readonly editItem = input<SomeDto | null>(null);
  readonly cancel = output<void>();
  readonly saved = output<void>();
  readonly submitting = signal(false);
  readonly submitError = signal<string | null>(null);
  readonly isEditMode = computed(() => this.editItem() !== null);

  protected close(): void {
    this.cancel.emit();
  }

  protected onFormSubmit(value: YourFormValue): void {
    this.submitError.set(null);
    this.submitting.set(true);

    const id = this.editItem()?.id;
    const request$ = id
      ? this.apiService.apiItemsIdPut(id, value)
      : this.apiService.apiItemsPost(value);

    request$.subscribe({
      next: () => {
        this.submitting.set(false);
        this.saved.emit();
      },
      error: () => {
        this.submitting.set(false);
        this.submitError.set(this.isEditMode() ? 'Could not update.' : 'Could not create.');
      },
    });
  }
}
```

```html
<!-- src/app/your-feature/your-popup/your-popup.html -->

<div class="popup-backdrop">
  <div class="popup card shadow border-0">
    <div class="card-header bg-white border-bottom">
      <h2 class="h5 mb-1 fw-bold">{{ isEditMode() ? 'Edit' : 'Create' }}</h2>
    </div>
    <div class="card-body">
      <app-your-form
        [editItem]="editItem()"
        [submitting]="submitting()"
        [submitError]="submitError()"
        [isEditMode]="isEditMode()"
        (formSubmit)="onFormSubmit($event)"
        (formCancel)="close()"
      />
    </div>
  </div>
</div>
```

---

## Common Validator Error Keys Reference

| Validator | Error key | Error object shape |
| --- | --- | --- |
| `Validators.required` | `required` | `{ required: true }` |
| `Validators.minLength(n)` | `minlength` | `{ requiredLength: N, actualLength: N }` |
| `Validators.maxLength(n)` | `maxlength` | `{ requiredLength: N, actualLength: N }` |
| `Validators.email` | `email` | `{ email: true }` |
| `Validators.min(n)` | `min` | `{ min: N, actual: N }` |
| `Validators.max(n)` | `max` | `{ max: N, actual: N }` |
| `Validators.pattern(regex)` | `pattern` | `{ requiredPattern: string, actualValue: string }` |
| Custom validator | your key | whatever you return from `ValidationErrors` |

---

## Writing Custom Validators

### Using `Validators.pattern` for character restrictions

Use `Validators.pattern` with a regex to restrict input to certain characters.

```ts
// Only digits and optional leading +
phone: ['', [Validators.minLength(5), Validators.maxLength(20), Validators.pattern(/^\+?[0-9]*$/)]],
```

```ts
{ key: 'pattern', message: 'Only digits and an optional leading + allowed.', showOnDirty: true },
```

### Single-field custom validator

A validator is a function `(control: AbstractControl) => ValidationErrors | null`.

```ts
import { AbstractControl, ValidationErrors } from '@angular/forms';

export function noWhitespace(control: AbstractControl): ValidationErrors | null {
  const value = control.value as string;
  if (value && value.trim().length === 0) {
    return { whitespace: true };
  }
  return null;
}
```

Usage in factory: `name: ['', [Validators.required, noWhitespace]]`

Usage in messages: `{ key: 'whitespace', message: 'Must not be only whitespace.', showOnDirty: true }`

### Cross-field validator

Cross-field validators are attached to the `FormGroup`, not individual controls.

```ts
static emailOrPhoneRequired(control: AbstractControl): ValidationErrors | null {
  const email = control.get('email')?.value?.trim() ?? '';
  const phone = control.get('phone')?.value?.trim() ?? '';
  if (!email && !phone) {
    return { emailOrPhoneRequired: true };
  }
  return null;
}
```

Attach in factory: `{ validators: [YourFactory.emailOrPhoneRequired] }`

Display in template (NOT via `<app-validate-input>`, since it applies to the group):

```html
@if (hasGroupError('emailOrPhoneRequired')) {
  <div class="validation-error">Either email or phone must be provided.</div>
}
```

The `hasGroupError` helper ensures the error only shows once at least one of the involved fields has been touched:

```ts
protected hasGroupError(errorKey: string): boolean {
  return this.form.hasError(errorKey) && (
    this.form.get('email')!.touched || this.form.get('phone')!.touched
  );
}
```

---

## Checklist for New Forms

Use this checklist to verify your form is complete:

- [ ] FormGroupFactory created with static `create(fb)` method
- [ ] All per-field validators attached in the factory (required, minLength, maxLength, email, pattern, etc.)
- [ ] Cross-field validators attached as group validators in the factory
- [ ] Form component creates form via `YourFormGroupFactory.create(this.fb)` in constructor
- [ ] `ValidationMessage[]` array defined for every validated field
- [ ] `showOnDirty: true` set on all format/length validators (minlength, maxlength, email, pattern)
- [ ] `showOnDirty` omitted or `false` for `required` validators
- [ ] Every validated input wrapped in `<app-validate-input [control]="..." [messages]="...">`
- [ ] `[class.is-invalid]` added to each input for Bootstrap red-border styling
- [ ] Required fields have `<span class="text-danger">*</span>` in label
- [ ] Cross-field errors shown via `@if (hasGroupError('key'))` outside `<app-validate-input>`
- [ ] Dropdowns load data in constructor with `signal` + `loadingSignal` pattern
- [ ] Dropdown shows "Loading..." while loading, "No selection" when loaded
- [ ] `submit()` calls `markAllAsTouched()` if form is invalid
- [ ] `submit()` trims all string values before emitting
- [ ] Parent component has no form/validation logic, only passes inputs and handles outputs
- [ ] `ReactiveFormsModule` and `ValidateInput` imported in component `imports` array

---

## Reference Implementation (Contact Form)

The contact form is the canonical example. Copy its structure for new forms.

| File | Path |
|---|---|
| FormGroupFactory | `src/app/contacts/contact-form/contact-form-group-factory.ts` |
| Form Component | `src/app/contacts/contact-form/contact-form.ts` |
| Form Template | `src/app/contacts/contact-form/contact-form.html` |
| Form Styles | `src/app/contacts/contact-form/contact-form.css` |
| Parent (Popup) | `src/app/contacts/contact-create-popup/contact-create-popup.ts` |
| Parent Template | `src/app/contacts/contact-create-popup/contact-create-popup.html` |
| ValidateInput | `src/app/shared/validate-input/validate-input.ts` |
