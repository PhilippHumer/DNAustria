import { Component, inject, signal } from '@angular/core';
import { EventsService } from '../api/api/events.service';
import { EventDto } from '../api/model/eventDto';
import { Filterevents } from "./filterevents/filterevents";
import { Eventcard } from "./eventcard/eventcard";
import { EventFormPopup } from "./event-form-popup/event-form-popup";

@Component({
  selector: 'app-events',
  imports: [Filterevents, Eventcard, EventFormPopup],
  templateUrl: './events.html',
  styleUrl: './events.css',
})
export class Events {
  private readonly eventsService = inject(EventsService);
  protected readonly events = signal<EventDto[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly isLoading = signal(false);
  protected readonly hasLoadedOnce = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly isFormOpen = signal(false);
  protected readonly editingEvent = signal<EventDto | null>(null);
  protected readonly eventToDelete = signal<EventDto | null>(null);
  protected readonly deleteInProgress = signal(false);
  protected readonly deleteError = signal<string | null>(null);

  constructor() {
    this.loadEvents();
  }

  protected openCreatePopup(): void {
    this.editingEvent.set(null);
    this.isFormOpen.set(true);
  }

  protected openEditPopup(event: EventDto): void {
    this.editingEvent.set(event);
    this.isFormOpen.set(true);
  }

  protected closeFormPopup(): void {
    this.isFormOpen.set(false);
    this.editingEvent.set(null);
  }

  protected handleEventSaved(): void {
    this.isFormOpen.set(false);
    this.editingEvent.set(null);
    this.loadEvents(this.searchTerm());
  }

  protected handleSearchTermChange(searchTerm: string): void {
    this.searchTerm.set(searchTerm);
    this.loadEvents(searchTerm);
  }

  protected openDeletePopup(event: EventDto): void {
    this.eventToDelete.set(event);
    this.deleteError.set(null);
  }

  protected closeDeletePopup(): void {
    this.eventToDelete.set(null);
    this.deleteError.set(null);
  }

  protected confirmDelete(): void {
    const id = this.eventToDelete()?.id;
    if (!id) {
      return;
    }

    this.deleteInProgress.set(true);
    this.deleteError.set(null);

    this.eventsService.apiEventsIdDelete(id).subscribe({
      next: () => {
        this.deleteInProgress.set(false);
        this.eventToDelete.set(null);
        this.loadEvents(this.searchTerm());
      },
      error: () => {
        this.deleteInProgress.set(false);
        this.deleteError.set('Event could not be deleted.');
      },
    });
  }

  private loadEvents(name?: string): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    const trimmedName = name?.trim() || undefined;
    this.eventsService.apiEventsGet(trimmedName).subscribe({
      next: (events) => {
        this.events.set(events ?? []);
        this.isLoading.set(false);
        this.hasLoadedOnce.set(true);
      },
      error: () => {
        this.events.set([]);
        this.loadError.set('Events could not be loaded.');
        this.isLoading.set(false);
        this.hasLoadedOnce.set(true);
      },
    });
  }
}
