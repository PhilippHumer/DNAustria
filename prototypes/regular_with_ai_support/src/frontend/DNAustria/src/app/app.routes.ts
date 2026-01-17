import { Routes } from '@angular/router';
import { Dashboard } from './dashboard/dashboard';
import { Events } from './events/events';
import { Contacts } from './contacts/contacts';
import { Organizations } from './organizations/organizations';
import { Export } from './export/export';

export const routes: Routes = 
[
    { path: '', component: Dashboard },
    { path: 'events', component: Events},
    { path: 'contacts', component: Contacts},
    { path: 'organizations', component: Organizations},
    { path: 'export', component: Export}
];
