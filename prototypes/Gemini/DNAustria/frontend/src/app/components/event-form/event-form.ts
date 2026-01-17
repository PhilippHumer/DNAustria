import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Event } from '../../models/event.model';
import { Organization } from '../../models/organization.model';
import { ApiService } from '../../services/api.service';
import { TargetAudience, EventTopic, EventClassification } from '../../models/enums';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    MatAutocompleteModule
  ],
  templateUrl: './event-form.html',
  styleUrl: './event-form.scss'
})
export class EventFormComponent implements OnInit, OnChanges {
  @Input() event?: Event | null;
  @Output() save = new EventEmitter<Event>();
  @Output() cancel = new EventEmitter<void>();

  form: FormGroup;
  organizations: Organization[] = [];
  
  targetAudiences = Object.values(TargetAudience).filter(value => typeof value === 'number');
  eventTopics = Object.values(EventTopic).filter(value => typeof value === 'number');
  classifications = Object.values(EventClassification).filter(value => typeof value === 'number');

  constructor(private fb: FormBuilder, private apiService: ApiService) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      eventLink: [''],
      organizationId: ['', Validators.required],
      targetAudience: [[]],
      topics: [[]],
      dateStart: [new Date(), Validators.required],
      dateEnd: [new Date(), Validators.required],
      classification: [EventClassification.Scheduled],
      fees: [false],
      isOnline: [false],
      // Address fields
      locationName: [''],
      city: [''],
      zip: [''],
      street: [''],
      // Contact fields
      contactName: [''],
      contactEmail: [''],
      contactPhone: ['']
    });
  }

  ngOnInit(): void {
    this.apiService.getOrganizations().subscribe(orgs => this.organizations = orgs);
    this.updateForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['event']) {
      this.updateForm();
    }
  }

  private updateForm(): void {
    if (this.event) {
      this.form.patchValue({
        ...this.event,
        locationName: this.event.location?.locationName,
        city: this.event.location?.city,
        zip: this.event.location?.zip,
        street: this.event.location?.street,
        contactName: this.event.contact?.name,
        contactEmail: this.event.contact?.email,
        contactPhone: this.event.contact?.phone
      });
    }
  }

  onSubmit() {
    if (this.form.valid) {
      const formValue = this.form.value;
      const event: Event = {
        ...this.event,
        ...formValue,
        location: {
            locationName: formValue.locationName,
            city: formValue.city,
            zip: formValue.zip,
            street: formValue.street,
            latitude: 0,
            longitude: 0
        },
        contact: {
            name: formValue.contactName,
            email: formValue.contactEmail,
            phone: formValue.contactPhone
        }
      };
      this.save.emit(event);
    }
  }
}
