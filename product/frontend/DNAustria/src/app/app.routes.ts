import { Routes } from '@angular/router';
import { Contacts } from './contacts/contacts';
import { Dashboard } from './dashboard/dashboard';
import { EventDetails } from './event-details/event-details';
import { Events } from './events/events';
import { Export } from './export/export';
import { Organizations } from './organizations/organizations';

export const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: Dashboard },
    { path: 'events', component: Events },
    { path: 'event-details', component: EventDetails },
    { path: 'contacts', component: Contacts },
    { path: 'organizations', component: Organizations },
    { path: 'export', component: Export },
    { path: '**', redirectTo: 'dashboard' }
];
