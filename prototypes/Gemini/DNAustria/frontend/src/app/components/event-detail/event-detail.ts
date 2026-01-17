import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../services/api.service';
import { Event } from '../../models/event.model';
import { EventFormComponent } from '../event-form/event-form';
import { EventStatus } from '../../models/enums';

import { Observable } from 'rxjs';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    EventFormComponent
  ],
  templateUrl: './event-detail.html',
  styleUrl: './event-detail.scss'
})
export class EventDetailComponent implements OnInit {
  event$?: Observable<Event>;
  isEditing = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadEvent(id);
    }
  }

  loadEvent(id: string) {
    this.event$ = this.apiService.getEvent(id);
  }

  onSave(event: Event) {
    if (event.id) {
      this.apiService.updateEvent(event.id, event).subscribe(() => {
        this.snackBar.open('Event updated', 'Close', { duration: 3000 });
        this.isEditing = false;
        this.loadEvent(event.id!);
      });
    }
  }

  onDelete(event: Event) {
    if (confirm('Are you sure you want to delete this event?')) {
      if (event.id) {
        this.apiService.deleteEvent(event.id).subscribe(() => {
          this.snackBar.open('Event deleted', 'Close', { duration: 3000 });
          this.router.navigate(['/events']);
        });
      }
    }
  }
}
