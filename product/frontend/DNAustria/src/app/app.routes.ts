import { Routes } from '@angular/router';
import { authGuard } from './auth/auth.guard';
import { Contacts } from './contacts/contacts';
import { Dashboard } from './dashboard/dashboard';
import { EventDetails } from './event-details/event-details';
import { Events } from './events/events';
import { LlmAnalyzer } from './llm-analyzer/llm-analyzer';
import { Login } from './login/login';
import { Organizations } from './organizations/organizations';
import { Locations } from './locations/locations';

export const routes: Routes = [
    { path: 'login', component: Login },
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
    { path: 'locations', component: Locations, canActivate: [authGuard] },
    { path: 'events', component: Events, canActivate: [authGuard] },
    { path: 'event-details/:id', component: EventDetails, canActivate: [authGuard] },
    { path: 'event-details', redirectTo: 'events' },
    { path: 'contacts', component: Contacts, canActivate: [authGuard] },
    { path: 'organizations', component: Organizations, canActivate: [authGuard] },
    { path: 'llm-analyzer', component: LlmAnalyzer, canActivate: [authGuard] },
    { path: '**', redirectTo: 'dashboard' }
];
