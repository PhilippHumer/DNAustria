import { Routes } from '@angular/router';
import { EventsListComponent } from './components/events-list.component';
import { ContactsListComponent } from './components/contacts-list.component';
import { OrganizationsListComponent } from './components/organizations-list.component';
import { PublicExportComponent } from './components/public-export.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'events' },
  { path: 'events', component: EventsListComponent },
  { path: 'contacts', component: ContactsListComponent },
  { path: 'organizations', component: OrganizationsListComponent },
  { path: 'export', component: PublicExportComponent }
];
