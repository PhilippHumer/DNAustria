import { DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { catchError, forkJoin, of } from 'rxjs';
import { ContactsService } from '../api/api/contacts.service';
import { EventsService } from '../api/api/events.service';
import { OrganizationsService } from '../api/api/organizations.service';
import { ContactDto } from '../api/model/contactDto';
import { EventDto } from '../api/model/eventDto';
import { OrganizationDto } from '../api/model/organizationDto';
import { getEventStatusBadgeClass, getEventStatusLabel } from '../events/event-utils';

@Component({
  selector: 'app-dashboard',
  imports: [DatePipe, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private static readonly RECENT_EVENTS_LIMIT = 3;

  private readonly eventsService = inject(EventsService);
  private readonly contactsService = inject(ContactsService);
  private readonly organizationsService = inject(OrganizationsService);

  protected readonly events = signal<EventDto[]>([]);
  protected readonly contacts = signal<ContactDto[]>([]);
  protected readonly organizations = signal<OrganizationDto[]>([]);
  protected readonly isLoading = signal(false);
  protected readonly loadError = signal<string | null>(null);

  protected readonly totalEvents = computed(() => this.events().length);
  protected readonly publishedEvents = computed(
    () => this.events().filter((event) => event.status === 1).length,
  );
  protected readonly totalContacts = computed(() => this.contacts().length);
  protected readonly totalOrganizations = computed(() => this.organizations().length);
  protected readonly recentEvents = computed(() =>
    [...this.events()]
      .sort((left, right) => this.toTimestamp(right.startDate) - this.toTimestamp(left.startDate))
      .slice(0, Dashboard.RECENT_EVENTS_LIMIT),
  );

  ngOnInit(): void {
    this.loadDashboardData();
  }

  protected eventStatusLabel(status: number): string {
    return getEventStatusLabel(status);
  }

  protected eventStatusBadgeClass(status: number): string {
    return getEventStatusBadgeClass(status);
  }

  private loadDashboardData(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    let hadLoadError = false;

    forkJoin({
      events: this.eventsService.apiEventsGet().pipe(
        catchError(() => {
          hadLoadError = true;
          return of([] as EventDto[]);
        }),
      ),
      contacts: this.contactsService.apiContactsGet().pipe(
        catchError(() => {
          hadLoadError = true;
          return of([] as ContactDto[]);
        }),
      ),
      organizations: this.organizationsService.apiOrganizationsGet().pipe(
        catchError(() => {
          hadLoadError = true;
          return of([] as OrganizationDto[]);
        }),
      ),
    }).subscribe(({ events, contacts, organizations }) => {
      this.events.set(Array.isArray(events) ? events : []);
      this.contacts.set(Array.isArray(contacts) ? contacts : []);
      this.organizations.set(Array.isArray(organizations) ? organizations : []);
      this.loadError.set(hadLoadError ? 'Some dashboard data could not be loaded.' : null);
      this.isLoading.set(false);
    });
  }

  private toTimestamp(value: string | null | undefined): number {
    if (!value) {
      return 0;
    }

    const timestamp = new Date(value).getTime();
    return Number.isNaN(timestamp) ? 0 : timestamp;
  }

}
