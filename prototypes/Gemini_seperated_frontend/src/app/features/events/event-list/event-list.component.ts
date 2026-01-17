import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';

import { EventService } from '../../../core/services/event.service';
import { Event, TargetAudience } from '../../../core/models/event.model';

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatChipsModule
  ],
  templateUrl: './event-list.component.html',
  styleUrl: './event-list.component.scss'
})
export class EventListComponent implements OnInit {
  events: Event[] = [];
  filteredEvents: Event[] = [];
  searchTerm: string = '';

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    this.eventService.getEvents().subscribe(data => {
      this.events = data;
      this.filterEvents();
    });
  }

  filterEvents(): void {
    if (!this.searchTerm) {
      this.filteredEvents = this.events;
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredEvents = this.events.filter(e => 
        e.title.toLowerCase().includes(term) || 
        e.description.toLowerCase().includes(term)
      );
    }
  }

  deleteEvent(event: Event, e: MouseEvent): void {
    e.stopPropagation(); // Prevent card click
    if (confirm(`Are you sure you want to delete "${event.title}"?`)) {
      this.eventService.deleteEvent(event.id).subscribe(() => {
        this.loadEvents();
      });
    }
  }

  getAudienceLabel(value: number | string): string {
    return (TargetAudience as any)[value] || value.toString();
  }
}
