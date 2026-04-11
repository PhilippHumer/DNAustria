import { Component, input, output } from '@angular/core';
import { EVENT_STATUS_OPTIONS } from '../event-utils';

@Component({
  selector: 'app-filterevents',
  imports: [],
  templateUrl: './filterevents.html',
  styleUrl: './filterevents.css',
})
export class Filterevents {
  protected readonly statusOptions = EVENT_STATUS_OPTIONS;

  readonly searchTerm = input('');
  readonly status = input<number | null>(null);
  readonly searchTermChange = output<string>();
  readonly statusChange = output<number | null>();

  protected onSearchTermChange(event: Event): void {
    const target = event.target as HTMLInputElement | null;
    this.searchTermChange.emit(target?.value ?? '');
  }

  protected onStatusChange(event: Event): void {
    const target = event.target as HTMLSelectElement | null;
    const value = target?.value ?? '';
    this.statusChange.emit(value === '' ? null : Number(value));
  }
}
