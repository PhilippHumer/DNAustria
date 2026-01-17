import { Routes } from '@angular/router';
import { EventListComponent } from './components/event-list/event-list';
import { EventDetailComponent } from './components/event-detail/event-detail';
import { EventCreateComponent } from './components/event-create/event-create';
import { ContactListComponent } from './components/contact-list/contact-list';
import { ContactDetailComponent } from './components/contact-detail/contact-detail';
import { OrganizationListComponent } from './components/organization-list/organization-list';
import { OrganizationDetailComponent } from './components/organization-detail/organization-detail';

export const routes: Routes = [
  { path: '', redirectTo: '/events', pathMatch: 'full' },
  { path: 'events', component: EventListComponent },
  { path: 'events/create', component: EventCreateComponent },
  { path: 'events/:id', component: EventDetailComponent },
  { path: 'contacts', component: ContactListComponent },
  { path: 'contacts/:id', component: ContactDetailComponent },
  { path: 'organizations', component: OrganizationListComponent },
  { path: 'organizations/:id', component: OrganizationDetailComponent }
];
