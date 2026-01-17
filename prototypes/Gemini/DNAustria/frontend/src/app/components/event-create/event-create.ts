import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Event } from '../../models/event.model';
import { EventFormComponent } from '../event-form/event-form';

@Component({
  selector: 'app-event-create',
  standalone: true,
  imports: [
    CommonModule,
    MatSnackBarModule,
    EventFormComponent,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    FormsModule
  ],
  templateUrl: './event-create.html',
  styleUrl: './event-create.scss'
})
export class EventCreateComponent {
  importedEvent: Event | null = null;
  importText: string = '';
  showImport: boolean = false;
  isImporting: boolean = false;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  toggleImport() {
    this.showImport = !this.showImport;
  }

  onImport() {
    if (!this.importText.trim()) return;

    this.isImporting = true;
    this.apiService.importEvent(this.importText).subscribe({
      next: (event: any) => {
        // Handle potential PascalCase from .NET backend
        const normalizedEvent: Event = {
          ...event,
          title: event.title || event.Title || '',
          description: event.description || event.Description || '',
          dateStart: event.dateStart || event.DateStart,
          dateEnd: event.dateEnd || event.DateEnd,
          eventLink: event.eventLink || event.EventLink,
          isOnline: event.isOnline || event.IsOnline,
          programName: event.programName || event.ProgramName,
          format: event.format || event.Format,
          ageMinimum: event.ageMinimum || event.AgeMinimum,
          ageMaximum: event.ageMaximum || event.AgeMaximum
        };
        
        this.importedEvent = normalizedEvent;
        this.isImporting = false;
        this.showImport = false;
        this.snackBar.open('Event imported successfully. Please review and save.', 'Close', { duration: 3000 });
      },
      error: (err) => {
        console.error('Error importing event', err);
        this.isImporting = false;
        this.snackBar.open('Error importing event', 'Close', { duration: 3000 });
      }
    });
  }

  onSave(event: Event) {
    this.apiService.createEvent(event).subscribe(createdEvent => {
      this.snackBar.open('Event created', 'Close', { duration: 3000 });
      this.router.navigate(['/events', createdEvent.id]);
    });
  }

  onCancel() {
    this.router.navigate(['/events']);
  }
}
