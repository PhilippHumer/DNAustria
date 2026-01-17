import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';

import { Observable, of } from 'rxjs';
import { map, startWith } from 'rxjs/operators';

import { EventService } from '../../../core/services/event.service';
import { OrganizationService } from '../../../core/services/organization.service';
import { EventStatus, EventClassification, TargetAudience, EventTopic } from '../../../core/models/event.model';
import { Address } from '../../../core/models/address.model';
import { Organization } from '../../../core/models/organization.model';

@Component({
  selector: 'app-event-create',
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
    MatAutocompleteModule,
    MatIconModule,
    MatSnackBarModule,
    MatTabsModule
  ],
  templateUrl: './event-create.component.html',
  styleUrl: './event-create.component.scss'
})
export class EventCreateComponent {
  eventForm: FormGroup;
  importForm: FormGroup;
  isImporting = false;
  
  // Enums for template
  targetAudiences = Object.keys(TargetAudience).filter(k => !isNaN(Number(k))).map(Number);
  eventTopics = Object.keys(EventTopic).filter(k => !isNaN(Number(k))).map(Number);
  eventStatuses = Object.keys(EventStatus).filter(k => !isNaN(Number(k))).map(Number);
  
  // Organizations
  organizations$: Observable<Organization[]>;

  // Dummy addresses for autocomplete
  addresses: Address[] = [
    { id: 'a1', name: 'Main Campus', city: 'Vienna', zip: '1010', state: 'Vienna', street: 'Ringstrasse 1', latitude: 48.2, longitude: 16.3 },
    { id: 'a2', name: 'Downtown Office', city: 'Vienna', zip: '1020', state: 'Vienna', street: 'Praterstrasse 5', latitude: 48.2, longitude: 16.3 },
  ];
  filteredAddresses: Observable<Address[]>;

  constructor(
    private fb: FormBuilder,
    private eventService: EventService,
    private organizationService: OrganizationService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.organizations$ = this.organizationService.getOrganizations();

    this.eventForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      organizationId: ['', Validators.required],
      dateStart: [new Date(), Validators.required],
      dateEnd: [new Date(), Validators.required],
      targetAudience: [[]],
      topics: [[]],
      status: [EventStatus.Draft, Validators.required],
      classification: [EventClassification.Scheduled],
      fees: [false],
      isOnline: [false],
      schoolBookable: [false],
      eventLink: [''],
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

    this.importForm = this.fb.group({
      unstructuredData: ['', Validators.required]
    });

    this.filteredAddresses = this.eventForm.get('location.name')!.valueChanges.pipe(
      startWith(''),
      map(value => this._filterAddresses(value || ''))
    );
  }

  private _filterAddresses(value: string): Address[] {
    const filterValue = value.toLowerCase();
    return this.addresses.filter(option => (option.name || '').toLowerCase().includes(filterValue));
  }

  onLocationSelected(event: any): void {
    const selectedLocation = this.addresses.find(a => a.name === event.option.value);
    if (selectedLocation) {
      this.eventForm.get('location')?.patchValue({
        street: selectedLocation.street,
        city: selectedLocation.city,
        zip: selectedLocation.zip,
        state: selectedLocation.state,
        latitude: selectedLocation.latitude,
        longitude: selectedLocation.longitude
      });
    }
  }

  getTargetAudienceLabel(value: number): string {
    return TargetAudience[value];
  }

  getTopicLabel(value: number): string {
    return EventTopic[value];
  }

  getStatusLabel(value: number): string {
    return EventStatus[value];
  }

  onImportSubmit(): void {
    if (this.importForm.valid) {
      this.isImporting = true;
      const data = this.importForm.get('unstructuredData')?.value;
      
      this.eventService.importEventData(data).subscribe({
        next: (extractedData) => {
          this.eventForm.patchValue(extractedData);
          this.isImporting = false;
          this.snackBar.open('Data imported successfully! Please review.', 'Close', { duration: 3000 });
        },
        error: () => {
          this.isImporting = false;
          this.snackBar.open('Failed to import data.', 'Close', { duration: 3000 });
        }
      });
    }
  }

  onSubmit(): void {
    if (this.eventForm.valid) {
      const formValue = this.eventForm.value;

      // Handle nullable UUIDs and empty strings
      if (!formValue.contactId) formValue.contactId = null;
      if (!formValue.organizationId) formValue.organizationId = null;
      if (!formValue.eventLink) formValue.eventLink = null;
      if (!formValue.programName) formValue.programName = null;
      if (!formValue.format) formValue.format = null;

      // Clean Contact: If no name is provided, assume no new contact is being created
      if (formValue.contact) {
        const c = formValue.contact;
        if (!c.name && !c.email && !c.phone && !c.org) {
          formValue.contact = null;
        } else {
           // Ensure empty strings in contact are nulls
           if (!c.name) c.name = null;
           if (!c.email) c.email = null;
           if (!c.phone) c.phone = null;
           if (!c.org) c.org = null;
        }
      }

      // Clean Location: If no name and no street, assume no location or reuse logic handled elsewhere
      // However, for creation, if fields are empty, sending null is cleaner than object with empty strings
      if (formValue.location) {
        const l = formValue.location;
        const hasLocationData = l.name || l.street || l.city || l.zip || l.state;
        
        if (!hasLocationData) {
           formValue.location = null;
        } else {
           l.latitude = Number(l.latitude) || 0;
           l.longitude = Number(l.longitude) || 0;
           if (!l.name) l.name = null;
           if (!l.street) l.street = null;
           if (!l.city) l.city = null;
           if (!l.zip) l.zip = null;
           if (!l.state) l.state = null;
        }
      }

      this.eventService.createEvent(formValue).subscribe({
        next: () => {
          this.snackBar.open('Event created successfully!', 'Close', { duration: 3000 });
          this.router.navigate(['/events']);
        },
        error: (err) => {
          console.error('Error creating event:', err);
          this.snackBar.open('Failed to create event. Please check your data.', 'Close', { duration: 5000 });
        }
      });
    }
  }
}
