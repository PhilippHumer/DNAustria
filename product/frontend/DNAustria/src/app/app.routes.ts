import { Routes } from '@angular/router';
import { Contacts } from './contacts/contacts';
import { Dashboard } from './dashboard/dashboard';
import { EventDetails } from './event-details/event-details';
import { Events } from './events/events';
import { Export } from './export/export';
import { LlmAnalyzer } from './llm-analyzer/llm-analyzer';
import { Organizations } from './organizations/organizations';
import { Locations } from './locations/locations';

export const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: Dashboard },
    { path: 'locations', component: Locations },
    { path: 'events', component: Events },
    { path: 'event-details/:id', component: EventDetails },
    { path: 'event-details', redirectTo: 'events' },
    { path: 'contacts', component: Contacts },
    { path: 'organizations', component: Organizations },
    { path: 'export', component: Export },
    { path: 'llm-analyzer', component: LlmAnalyzer },
    { path: '**', redirectTo: 'dashboard' }
];
