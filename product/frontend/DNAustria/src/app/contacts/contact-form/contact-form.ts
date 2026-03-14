import { Component, effect, inject, input, output, signal } from '@angular/core';
import { FormGroup, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { ContactDto } from '../../api/model/contactDto';
import { OrganizationsService } from '../../api/api/organizations.service';
import { ContactFormGroupFactory } from './contact-form-group-factory';
import { ValidateInput, ValidationMessage } from '../../shared/validate-input/validate-input';

export interface ContactFormValue {
  name: string;
  email: string;
  phone: string;
  organization: string;
}

@Component({
  selector: 'app-contact-form',
  imports: [ReactiveFormsModule, ValidateInput],
  templateUrl: './contact-form.html',
  styleUrl: './contact-form.css',
})
export class ContactForm {
  private readonly fb = inject(FormBuilder);
  private readonly organizationsService = inject(OrganizationsService);

  readonly editContact = input<ContactDto | null>(null);
  readonly organizations = signal<string[]>([]);
  readonly organizationsLoading = signal(false);
  readonly submitting = input(false);
  readonly submitError = input<string | null>(null);
  readonly isEditMode = input(false);

  readonly formSubmit = output<ContactFormValue>();
  readonly formCancel = output<void>();

  readonly form: FormGroup;

  readonly nameMessages: ValidationMessage[] = [
    { key: 'required', message: 'Name is required.' },
    { key: 'minlength', message: 'Name must be at least 2 characters.', showOnDirty: true },
    { key: 'maxlength', message: 'Name must not exceed 50 characters.', showOnDirty: true },
  ];

  readonly emailMessages: ValidationMessage[] = [
    { key: 'email', message: 'Please enter a valid email address.', showOnDirty: true },
  ];

  readonly phoneMessages: ValidationMessage[] = [
    { key: 'minlength', message: 'Phone number must be at least 5 characters.', showOnDirty: true },
    { key: 'maxlength', message: 'Phone number must not exceed 20 characters.', showOnDirty: true },
    { key: 'pattern', message: 'Phone number may only contain digits and an optional leading +.', showOnDirty: true },
  ];

  constructor() {
    this.form = ContactFormGroupFactory.create(this.fb);
    this.loadOrganizations();

    effect(() => {
      const contact = this.editContact();
      if (contact) {
        this.form.setValue({
          name: contact.name ?? '',
          email: contact.email ?? '',
          phone: contact.phone ?? '',
          organization: contact.organization ?? '',
        });
        return;
      }

      this.form.reset({
        name: '',
        email: '',
        phone: '',
        organization: '',
      });
    });
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.formSubmit.emit({
      name: value.name.trim(),
      email: value.email.trim(),
      phone: value.phone.trim(),
      organization: value.organization.trim(),
    });
  }

  protected cancel(): void {
    this.formCancel.emit();
  }

  private loadOrganizations(): void {
    this.organizationsLoading.set(true);
    this.organizationsService.apiOrganizationsGet().subscribe({
      next: (result) => {
        if (!Array.isArray(result)) {
          this.organizations.set([]);
          return;
        }
        const names = result
          .map((item: { name?: unknown }) => item.name)
          .filter((name: unknown): name is string => typeof name === 'string' && name.length > 0);
        this.organizations.set(names);
        this.organizationsLoading.set(false);
      },
      error: () => {
        this.organizations.set([]);
        this.organizationsLoading.set(false);
      },
    });
  }

  protected hasGroupError(errorKey: string): boolean {
    return this.form.hasError(errorKey) && (this.form.get('email')!.touched || this.form.get('phone')!.touched);
  }
}
