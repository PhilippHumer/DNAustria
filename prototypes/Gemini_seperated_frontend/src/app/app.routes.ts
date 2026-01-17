import { Routes } from '@angular/router';

import { EventListComponent } from './features/events/event-list/event-list.component';
import { EventCreateComponent } from './features/events/event-create/event-create.component';
import { EventDetailComponent } from './features/events/event-detail/event-detail.component';

import { ContactListComponent } from './features/contacts/contact-list/contact-list.component';
import { ContactCreateComponent } from './features/contacts/contact-create/contact-create.component';
import { ContactDetailComponent } from './features/contacts/contact-detail/contact-detail.component';

import { OrganizationListComponent } from './features/organizations/organization-list/organization-list.component';
import { OrganizationCreateComponent } from './features/organizations/organization-create/organization-create.component';
import { OrganizationDetailComponent } from './features/organizations/organization-detail/organization-detail.component';

export const routes: Routes = [
  { path: '', redirectTo: 'events', pathMatch: 'full' },
  
  // Events
  { path: 'events', component: EventListComponent },
  { path: 'events/create', component: EventCreateComponent },
  { path: 'events/:id', component: EventDetailComponent },
  
  // Contacts
  { path: 'contacts', component: ContactListComponent },
  { path: 'contacts/create', component: ContactCreateComponent },
  { path: 'contacts/:id', component: ContactDetailComponent },
  
  // Organizations
  { path: 'organizations', component: OrganizationListComponent },
  { path: 'organizations/create', component: OrganizationCreateComponent },
  { path: 'organizations/:id', component: OrganizationDetailComponent },
  
  { path: '**', redirectTo: 'events' }
];
