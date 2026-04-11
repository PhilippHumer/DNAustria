import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
  private static readonly PAGE_SIZE = 20;

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly eventsService = inject(EventsService);
  protected readonly events = signal<EventDto[]>([]);
  protected readonly searchTerm = signal('');
  protected readonly status = signal<number | null>(null);
  protected readonly currentPage = signal(1);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = signal(0);
  protected readonly isLoading = signal(false);
  protected readonly hasLoadedOnce = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly isFormOpen = signal(false);
  protected readonly editingEvent = signal<EventDto | null>(null);
  protected readonly eventToDelete = signal<EventDto | null>(null);
  protected readonly deleteInProgress = signal(false);
  protected readonly deleteError = signal<string | null>(null);
  protected readonly pageNumbers = computed(() =>
    Array.from({ length: this.totalPages() }, (_, index) => index + 1),
  );

  constructor() {
    this.route.queryParamMap.subscribe((params) => {
      const searchTerm = (params.get('name') ?? '').trim();
      const status = this.parseStatus(params.get('status'));
      const page = this.parsePage(params.get('page'));

      this.searchTerm.set(searchTerm);
      this.status.set(status);
      this.currentPage.set(page);
      this.loadEvents(searchTerm, status, page);
    });
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
    this.loadEvents(this.searchTerm(), this.status(), this.currentPage());
  }

  protected handleSearchTermChange(searchTerm: string): void {
    this.updateQueryParams(searchTerm, this.status(), 1);
  }

  protected handleStatusChange(status: number | null): void {
    this.updateQueryParams(this.searchTerm(), status, 1);
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.currentPage()) {
      return;
    }

    this.updateQueryParams(this.searchTerm(), this.status(), page);
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
        const nextPage = this.events().length === 1 && this.currentPage() > 1
          ? this.currentPage() - 1
          : this.currentPage();
        this.updateQueryParams(this.searchTerm(), this.status(), nextPage);
      },
      error: () => {
        this.deleteInProgress.set(false);
        this.deleteError.set('Event could not be deleted.');
      },
    });
  }

  private loadEvents(name?: string, status?: number | null, page: number = 1): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    const trimmedName = name?.trim() || undefined;
    this.eventsService.apiEventsGet(trimmedName, status ?? undefined, page, Events.PAGE_SIZE).subscribe({
      next: (response) => {
        this.events.set(response?.items ?? []);
        this.totalCount.set(response?.totalCount ?? 0);
        this.totalPages.set(response?.totalPages ?? 0);
        this.currentPage.set(response?.page ?? page);
        this.isLoading.set(false);
        this.hasLoadedOnce.set(true);
      },
      error: () => {
        this.events.set([]);
        this.totalCount.set(0);
        this.totalPages.set(0);
        this.loadError.set('Events could not be loaded.');
        this.isLoading.set(false);
        this.hasLoadedOnce.set(true);
      },
    });
  }

  private updateQueryParams(name: string, status: number | null, page: number): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        name: name.trim() || null,
        status,
        page: page > 1 ? page : null,
      },
    });
  }

  private parseStatus(value: string | null): number | null {
    if (value === null || value === '') {
      return null;
    }

    const parsed = Number(value);
    return Number.isInteger(parsed) ? parsed : null;
  }

  private parsePage(value: string | null): number {
    const parsed = Number(value);
    return Number.isInteger(parsed) && parsed > 0 ? parsed : 1;
  }
}
