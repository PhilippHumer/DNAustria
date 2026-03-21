import { Component, inject, signal } from '@angular/core';
import type { HttpErrorResponse } from '@angular/common/http';
import { ContactsService } from '../api/api/contacts.service';
import { ContactDto } from '../api/model/contactDto';
import { Contactcard } from "./contactcard/contactcard";
import { ContactCreatePopup } from "./contact-create-popup/contact-create-popup";
import { ConfirmDeletePopup } from "../shared/confirm-delete-popup/confirm-delete-popup";
import { getErrorText } from '../shared/get-error-text';

@Component({
  selector: 'app-contacts',
  imports: [Contactcard, ContactCreatePopup, ConfirmDeletePopup],
  templateUrl: './contacts.html',
  styleUrl: './contacts.css',
})
export class Contacts {
  private readonly contactsService = inject(ContactsService);
  protected readonly contacts = signal<ContactDto[]>([]);
  protected readonly isCreatePopupOpen = signal(false);
  protected readonly editingContact = signal<ContactDto | null>(null);
  protected readonly contactToDelete = signal<ContactDto | null>(null);
  protected readonly deleteInProgress = signal(false);
  protected readonly deleteError = signal<string | null>(null);

  constructor() {
    this.loadContacts();
  }

  protected openCreatePopup(): void {
    this.editingContact.set(null);
    this.isCreatePopupOpen.set(true);
  }

  protected closeCreatePopup(): void {
    this.isCreatePopupOpen.set(false);
    this.editingContact.set(null);
  }

  protected openEditPopup(contact: ContactDto): void {
    this.editingContact.set(contact);
    this.isCreatePopupOpen.set(true);
  }

  protected openDeletePopup(contact: ContactDto): void {
    this.contactToDelete.set(contact);
    this.deleteError.set(null);
  }

  protected closeDeletePopup(): void {
    this.contactToDelete.set(null);
    this.deleteError.set(null);
  }

  protected handleContactSaved(): void {
    this.isCreatePopupOpen.set(false);
    this.editingContact.set(null);
    this.loadContacts();
  }

  protected confirmDelete(): void {
    const id = this.contactToDelete()?.id;
    if (!id) {
      return;
    }

    this.deleteInProgress.set(true);
    this.deleteError.set(null);

    this.contactsService.apiContactsIdDelete(id).subscribe({
      next: () => {
        this.deleteInProgress.set(false);
        this.contactToDelete.set(null);
        this.loadContacts();
      },
      error: (err: HttpErrorResponse) => {
        this.deleteInProgress.set(false);
        this.deleteError.set(getErrorText(err) || 'Contact could not be deleted.');
      },
    });
  }

  private loadContacts(): void {
    this.contactsService.apiContactsGet().subscribe((contacts) => {
      this.contacts.set(contacts ?? []);
    });
  }
}
