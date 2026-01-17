import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { ContactService } from '../../../core/services/contact.service';
import { Contact } from '../../../core/models/contact.model';

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatButtonModule, MatIconModule],
  templateUrl: './contact-list.component.html',
  styleUrl: './contact-list.component.scss'
})
export class ContactListComponent implements OnInit {
  contacts: Contact[] = [];
  displayedColumns: string[] = ['name', 'email', 'phone', 'org', 'actions'];

  constructor(private contactService: ContactService) {}

  ngOnInit(): void {
    this.contactService.getContacts().subscribe(data => this.contacts = data);
  }

  deleteContact(id: string, event: Event): void {
    event.stopPropagation();
    if (confirm('Delete contact?')) {
        this.contactService.deleteContact(id).subscribe(() => {
            this.contactService.getContacts().subscribe(data => this.contacts = data);
        });
    }
  }
}
