import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';

import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { EventService } from '../../../core/services/event.service';
import { OrganizationService } from '../../../core/services/organization.service';
import { Event, EventStatus, EventClassification, TargetAudience, EventTopic } from '../../../core/models/event.model';
import { Organization } from '../../../core/models/organization.model';

@Component({
  selector: 'app-event-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    MatCheckboxModule,
    MatIconModule,
    MatMenuModule,
    MatSnackBarModule
  ],
  templateUrl: './event-detail.component.html',
  styleUrl: './event-detail.component.scss'
})
export class EventDetailComponent implements OnInit {
  eventForm: FormGroup;
  eventId: string | null = null;
  currentEvent: Event | null = null;
  organization: Organization | null = null;

  targetAudiences = Object.values(TargetAudience).filter(value => typeof value === 'number');
  eventTopics = Object.values(EventTopic).filter(value => typeof value === 'number');
  eventStatuses = Object.values(EventStatus).filter(value => typeof value === 'number');
  eventClassifications = Object.values(EventClassification).filter(value => typeof value === 'number');

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private organizationService: OrganizationService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      eventLink: [''],
      dateStart: [null, Validators.required],
      dateEnd: [null, Validators.required],
      targetAudience: [[]],
      topics: [[]],
      status: [EventStatus.Draft, Validators.required],
      classification: [EventClassification.Scheduled],
      fees: [false],
      isOnline: [false],
      schoolBookable: [false],
      organizationId: [''],
      programName: [''],
      format: [''],
      ageMinimum: [0],
      ageMaximum: [0],
      location: this.fb.group({
        name: [''],
        street: [''],
        city: [''],
        zip: [''],
        state: [''],
        latitude: [0],
        longitude: [0]
      }),
      contactId: [''],
      contact: this.fb.group({
        name: [''],
        org: [''],
        email: [''],
        phone: ['']
      })
    });
  }

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('id');
    if (this.eventId) {
      this.loadEvent(this.eventId);
    }
  }

  loadEvent(id: string): void {
    this.eventService.getEvent(id).subscribe(event => {
      if (event) {
        this.currentEvent = event;
        // Convert date strings to Date objects for Material Datepicker
        const formValue = {
          ...event,
          dateStart: event.dateStart ? new Date(event.dateStart) : null,
          dateEnd: event.dateEnd ? new Date(event.dateEnd) : null
        };
        this.eventForm.patchValue(formValue);
        
        if (event.organizationId) {
            this.organizationService.getOrganization(event.organizationId).subscribe(org => {
                this.organization = org;
            });
        }

        if (event.status === EventStatus.Transferred) {
          this.eventForm.disable(); // Read-only if transferred
        }
      } else {
        this.snackBar.open('Event not found', 'Close');
        this.router.navigate(['/events']);
      }
    });
  }

  updateStatus(status: EventStatus): void {
    if (!this.eventId || !this.currentEvent) return;

    if (confirm(`Change status to ${status}?`)) {
      this.eventService.updateEvent(this.eventId, { status }).subscribe(updated => {
        this.currentEvent = updated;
        this.eventForm.patchValue({ status: updated.status });
        this.snackBar.open('Status updated', 'Close', { duration: 2000 });
        
        if (updated.status === EventStatus.Transferred) {
            this.eventForm.disable();
        } else {
            this.eventForm.enable();
        }
      });
    }
  }

  onUpdate(): void {
    if (this.eventForm.valid && this.eventId) {
      if (confirm('Update event details?')) {
        this.eventService.updateEvent(this.eventId, this.eventForm.value).subscribe(() => {
            this.snackBar.open('Event updated!', 'Close', { duration: 3000 });
            this.router.navigate(['/events']);
        });
      }
    }
  }

  onDelete(): void {
    if (this.eventId) {
      if (confirm('Are you sure you want to delete this event? This action cannot be undone.')) {
        this.eventService.deleteEvent(this.eventId).subscribe(() => {
            this.snackBar.open('Event deleted', 'Close', { duration: 3000 });
            this.router.navigate(['/events']);
        });
      }
    }
  }

  getStatusLabel(status: any): string {
    return EventStatus[status] || status;
  }

  getClassificationLabel(val: any): string {
    return EventClassification[val] || val;
  }

  getTargetAudienceLabel(val: any): string {
    return TargetAudience[val] || val;
  }

  getTopicLabel(val: any): string {
    return EventTopic[val] || val;
  }
}
