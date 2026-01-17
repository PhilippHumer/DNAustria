import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../services/api.service';
import { Contact } from '../../models/contact.model';

import { Observable } from 'rxjs';

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './contact-list.html',
  styleUrl: './contact-list.scss'
})
export class ContactListComponent implements OnInit {
  contacts$!: Observable<Contact[]>;
  displayedColumns: string[] = ['name', 'email', 'phone', 'org'];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.contacts$ = this.apiService.getContacts();
  }
}
