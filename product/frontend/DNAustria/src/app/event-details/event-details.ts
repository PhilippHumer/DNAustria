import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ContactsService } from '../api/api/contacts.service';
import { EventsService } from '../api/api/events.service';
import { LocationsService } from '../api/api/locations.service';
import { OrganizationsService } from '../api/api/organizations.service';
import { ContactDto } from '../api/model/contactDto';
import { EventDto } from '../api/model/eventDto';
import { EventHistoryDto } from '../api/model/eventHistoryDto';
import { LocationReplyDto } from '../api/model/locationReplyDto';
import { OrganizationDto } from '../api/model/organizationDto';
import { EventFormPopup } from '../events/event-form-popup/event-form-popup';
import {
  getEventClassificationLabel,
  getEventStatusBadgeClass,
  getEventStatusLabel,
  getEventTargetAudienceOption,
  getEventTopicOption,
} from '../events/event-utils';

@Component({
  selector: 'app-event-details',
  imports: [DatePipe, RouterLink, EventFormPopup],
  templateUrl: './event-details.html',
  styleUrl: './event-details.css',
})
export class EventDetails {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly eventsService = inject(EventsService);
  private readonly organizationsService = inject(OrganizationsService);
  private readonly contactsService = inject(ContactsService);
  private readonly locationsService = inject(LocationsService);

  protected readonly event = signal<EventDto | null>(null);
  protected readonly organization = signal<OrganizationDto | null>(null);
  protected readonly contact = signal<ContactDto | null>(null);
  protected readonly location = signal<LocationReplyDto | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly isFormOpen = signal(false);
  protected readonly isDeleting = signal(false);
  protected readonly deleteError = signal<string | null>(null);
  protected readonly isDeleteDialogOpen = signal(false);
  protected readonly sortedHistory = computed(() =>
    [...(this.event()?.history ?? [])].sort(
      (left, right) => new Date(right.createdAt).getTime() - new Date(left.createdAt).getTime(),
    ),
  );

  constructor() {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (!Number.isInteger(id) || id <= 0) {
        this.loadError.set('Invalid event ID.');
        this.isLoading.set(false);
        return;
      }

      this.loadEvent(id);
    });
  }

  protected statusLabel(status: number): string {
    return getEventStatusLabel(status);
  }

  protected statusBadgeClass(status: number): string {
    return getEventStatusBadgeClass(status);
  }

  protected classificationLabel(classification: number): string {
    return getEventClassificationLabel(classification);
  }

  protected topicOption(topic: number) {
    return getEventTopicOption(topic);
  }

  protected targetAudienceOption(audience: number) {
    return getEventTargetAudienceOption(audience);
  }

  protected historyEntryLabel(entry: EventHistoryDto): string {
    return `${entry.action} by ${entry.username}`;
  }

  protected openEditPopup(): void {
    this.isFormOpen.set(true);
  }

  protected closeFormPopup(): void {
    this.isFormOpen.set(false);
  }

  protected handleSaved(): void {
    const id = this.event()?.id;
    this.isFormOpen.set(false);
    if (id) {
      this.loadEvent(id);
    }
  }

  protected goBack(): void {
    this.router.navigate(['/events']);
  }

  protected openDeleteDialog(): void {
    this.isDeleteDialogOpen.set(true);
    this.deleteError.set(null);
  }

  protected closeDeleteDialog(): void {
    this.isDeleteDialogOpen.set(false);
    this.deleteError.set(null);
  }

  protected confirmDelete(): void {
    const id = this.event()?.id;
    if (!id) {
      return;
    }

    this.isDeleting.set(true);
    this.deleteError.set(null);

    this.eventsService.apiEventsIdDelete(id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.router.navigate(['/events']);
      },
      error: () => {
        this.isDeleting.set(false);
        this.deleteError.set('Event could not be deleted.');
      },
    });
  }

  private loadEvent(id: number): void {
    this.isLoading.set(true);
    this.loadError.set(null);
    this.organization.set(null);
    this.contact.set(null);
    this.location.set(null);

    this.eventsService.apiEventsIdGet(id).subscribe({
      next: (event) => {
        this.event.set(event);
        this.isLoading.set(false);
        this.loadRelatedData(event);
      },
      error: () => {
        this.event.set(null);
        this.loadError.set('Event could not be loaded.');
        this.isLoading.set(false);
      },
    });
  }

  private loadRelatedData(event: EventDto): void {
    if (event.organization) {
      this.organizationsService.apiOrganizationsIdGet(event.organization).subscribe({
        next: (organization: OrganizationDto) => this.organization.set(organization),
        error: () => this.organization.set(null),
      });
    }

    if (event.contact) {
      this.contactsService.apiContactsIdGet(event.contact).subscribe({
        next: (contact) => this.contact.set(contact),
        error: () => this.contact.set(null),
      });
    }

    if (event.location) {
      this.locationsService.locationsIdGet(event.location).subscribe({
        next: (location) => this.location.set(location),
        error: () => this.location.set(null),
      });
    }
  }
}
