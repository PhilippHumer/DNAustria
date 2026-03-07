import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ContactsService } from '../../api/api/contacts.service';
import { OrganizationsService } from '../../api/api/organizations.service';
import { ContactDto } from '../../api/model/contactDto';
import { CreateContactDto } from '../../api/model/createContactDto';
import { UpdateContactDto } from '../../api/model/updateContactDto';

@Component({
  selector: 'app-contact-create-popup',
  imports: [ReactiveFormsModule],
  templateUrl: './contact-create-popup.html',
  styleUrl: './contact-create-popup.css',
})
export class ContactCreatePopup {
  private readonly formBuilder = inject(FormBuilder);
  private readonly contactsService = inject(ContactsService);
  private readonly organizationsService = inject(OrganizationsService);

  readonly editContact = input<ContactDto | null>(null);
  readonly cancel = output<void>();
  readonly saved = output<void>();
  readonly organizations = signal<string[]>([]);
  readonly submitting = signal(false);
  readonly submitError = signal<string | null>(null);
  readonly isEditMode = computed(() => this.editContact() !== null);

  readonly form = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required]],
    email: ['', [Validators.email]],
    phone: [''],
    organization: [''],
  });

  constructor() {
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

  protected close(): void {
    this.cancel.emit();
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const name = value.name.trim();
    const email = value.email.trim() ? value.email.trim() : null;
    const phone = value.phone.trim() ? value.phone.trim() : null;
    const organization = value.organization.trim() ? value.organization.trim() : null;

    const createPayload: CreateContactDto = {
      name,
      email,
      phone,
      organization,
    };

    const updatePayload: UpdateContactDto = {
      name,
      email,
      phone,
      organization,
    };

    this.submitError.set(null);
    this.submitting.set(true);

    const contactId = this.editContact()?.id;
    const request$ = contactId
      ? this.contactsService.apiContactsIdPut(contactId, updatePayload)
      : this.contactsService.apiContactsPost(createPayload);

    request$.subscribe({
      next: () => {
        this.submitting.set(false);
        this.saved.emit();
      },
      error: () => {
        this.submitting.set(false);
        this.submitError.set(this.isEditMode() ? 'Contact could not be updated.' : 'Contact could not be created.');
      },
    });
  }

  private loadOrganizations(): void {
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
      },
      error: () => {
        this.organizations.set([]);
      },
    });
  }
}
