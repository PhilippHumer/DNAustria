import { Component, computed, inject, input, output, signal } from '@angular/core';
import { ContactsService } from '../../api/api/contacts.service';
import { ContactDto } from '../../api/model/contactDto';
import { CreateContactDto } from '../../api/model/createContactDto';
import { UpdateContactDto } from '../../api/model/updateContactDto';
import { ContactForm, ContactFormValue } from '../contact-form/contact-form';

@Component({
  selector: 'app-contact-create-popup',
  imports: [ContactForm],
  templateUrl: './contact-create-popup.html',
  styleUrl: './contact-create-popup.css',
})
export class ContactCreatePopup {
  private readonly contactsService = inject(ContactsService);

  readonly editContact = input<ContactDto | null>(null);
  readonly cancel = output<void>();
  readonly saved = output<void>();
  readonly submitting = signal(false);
  readonly submitError = signal<string | null>(null);
  readonly isEditMode = computed(() => this.editContact() !== null);

  protected close(): void {
    this.cancel.emit();
  }

  protected onFormSubmit(value: ContactFormValue): void {
    const name = value.name;
    const email = value.email || null;
    const phone = value.phone || null;
    const organization = value.organization || null;

    const createPayload: CreateContactDto = { name, email, phone, organization };
    const updatePayload: UpdateContactDto = { name, email, phone, organization };

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

}
