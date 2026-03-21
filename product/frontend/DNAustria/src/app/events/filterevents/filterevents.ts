import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-filterevents',
  imports: [],
  templateUrl: './filterevents.html',
  styleUrl: './filterevents.css',
})
export class Filterevents {
  readonly searchTerm = input('');
  readonly searchTermChange = output<string>();

  protected onSearchTermChange(event: Event): void {
    const target = event.target as HTMLInputElement | null;
    this.searchTermChange.emit(target?.value ?? '');
  }
}
