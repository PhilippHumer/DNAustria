import { Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ContactsService } from '../../api/api/contacts.service';
import { EventsService } from '../../api/api/events.service';
import { LocationsService } from '../../api/api/locations.service';
import { OrganizationsService } from '../../api/api/organizations.service';
import { ContactDto } from '../../api/model/contactDto';
import { EventDto } from '../../api/model/eventDto';
import { InsertEventDto } from '../../api/model/insertEventDto';
import { LocationReplyDto } from '../../api/model/locationReplyDto';
import { OrganizationDto } from '../../api/model/organizationDto';
import { UpdateEventDto } from '../../api/model/updateEventDto';
import {
  EVENT_CLASSIFICATION_OPTIONS,
  EVENT_STATUS_OPTIONS,
  EVENT_TARGET_AUDIENCE_OPTIONS,
  EVENT_TOPIC_OPTIONS,
  fromDatetimeLocalValue,
  toDatetimeLocalValue,
} from '../event-utils';

@Component({
  selector: 'app-event-form-popup',
  imports: [ReactiveFormsModule],
  templateUrl: './event-form-popup.html',
  styleUrl: './event-form-popup.css',
})
export class EventFormPopup {
  private readonly formBuilder = inject(FormBuilder);
  private readonly eventsService = inject(EventsService);
  private readonly organizationsService = inject(OrganizationsService);
  private readonly contactsService = inject(ContactsService);
  private readonly locationsService = inject(LocationsService);

  readonly editEvent = input<EventDto | null>(null);
  readonly cancel = output<void>();
  readonly saved = output<void>();

  readonly organizations = signal<OrganizationDto[]>([]);
  readonly contacts = signal<ContactDto[]>([]);
  readonly locations = signal<LocationReplyDto[]>([]);
  readonly submitting = signal(false);
  readonly submitError = signal<string | null>(null);
  readonly isEditMode = computed(() => this.editEvent() !== null);
  protected readonly classificationOptions = EVENT_CLASSIFICATION_OPTIONS;
  protected readonly statusOptions = EVENT_STATUS_OPTIONS;
  protected readonly targetAudienceOptions = EVENT_TARGET_AUDIENCE_OPTIONS;
  protected readonly topicOptions = EVENT_TOPIC_OPTIONS;

  readonly form = this.formBuilder.nonNullable.group(
    {
      name: ['', [this.trimmedRequiredValidator(), Validators.maxLength(100)]],
      description: ['', [this.trimmedRequiredValidator(), Validators.maxLength(2000)]],
      link: ['', [this.trimmedRequiredValidator()]],
      startDate: ['', [Validators.required]],
      endDate: ['', [Validators.required]],
      classification: [0, [Validators.required]],
      status: [0, [Validators.required]],
      hasFees: [false],
      isOnline: [false],
      organization: ['', [Validators.required]],
      programName: [''],
      format: [''],
      schoolBookable: [false],
      ageMinimum: [0, [Validators.min(0)]],
      ageMaximum: [999, [Validators.min(0)]],
      location: [''],
      contact: [''],
      targetAudiences: this.formBuilder.nonNullable.control<number[]>([], [this.requiredNumberArrayValidator()]),
      topics: this.formBuilder.nonNullable.control<number[]>([], [this.requiredNumberArrayValidator()]),
    },
    {
      validators: [this.dateRangeValidator(), this.ageRangeValidator()],
    },
  );

  constructor() {
    this.loadRelatedData();

    effect(() => {
      const event = this.editEvent();
      if (event) {
        this.form.reset({
          name: event.name ?? '',
          description: event.description ?? '',
          link: event.link ?? '',
          startDate: toDatetimeLocalValue(event.startDate),
          endDate: toDatetimeLocalValue(event.endDate),
          classification: event.classification ?? 0,
          status: event.status ?? 0,
          hasFees: event.hasFees ?? false,
          isOnline: event.isOnline ?? false,
          organization: event.organization?.toString() ?? '',
          programName: event.programName ?? '',
          format: event.format ?? '',
          schoolBookable: event.schoolBookable ?? false,
          ageMinimum: event.ageMinimum ?? 0,
          ageMaximum: event.ageMaximum ?? 999,
          location: event.location?.toString() ?? '',
          contact: event.contact?.toString() ?? '',
          targetAudiences: event.targetAudiences ?? [],
          topics: event.topics ?? [],
        });
        return;
      }

      this.form.reset({
        name: '',
        description: '',
        link: '',
        startDate: '',
        endDate: '',
        classification: 0,
        status: 0,
        hasFees: false,
        isOnline: false,
        organization: '',
        programName: '',
        format: '',
        schoolBookable: false,
        ageMinimum: 0,
        ageMaximum: 999,
        location: '',
        contact: '',
        targetAudiences: [],
        topics: [],
      });
    });
  }

  protected close(): void {
    this.cancel.emit();
  }

  protected hasControlError(controlName: keyof typeof this.form.controls, errorKey: string): boolean {
    const control = this.form.controls[controlName];
    return control.touched && control.hasError(errorKey);
  }

  protected hasFormError(errorKey: string): boolean {
    return this.form.hasError(errorKey) && (this.form.controls.startDate.touched || this.form.controls.endDate.touched || this.form.controls.ageMinimum.touched || this.form.controls.ageMaximum.touched);
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const payloadBase: InsertEventDto = {
      name: value.name.trim(),
      description: value.description.trim(),
      link: value.link.trim(),
      startDate: fromDatetimeLocalValue(value.startDate),
      endDate: fromDatetimeLocalValue(value.endDate),
      classification: Number(value.classification),
      status: Number(value.status),
      hasFees: value.hasFees,
      isOnline: value.isOnline,
      organization: value.organization ? Number(value.organization) : null,
      programName: value.programName.trim(),
      format: value.format.trim(),
      schoolBookable: value.schoolBookable,
      ageMinimum: Number(value.ageMinimum),
      ageMaximum: Number(value.ageMaximum),
      location: value.location ? Number(value.location) : null,
      contact: value.contact ? Number(value.contact) : null,
      targetAudiences: value.targetAudiences,
      topics: value.topics,
    };

    const updatePayload: UpdateEventDto = { ...payloadBase };

    this.submitError.set(null);
    this.submitting.set(true);

    const eventId = this.editEvent()?.id;
    const request$ = eventId
      ? this.eventsService.apiEventsIdPut(eventId, updatePayload)
      : this.eventsService.apiEventsPost(payloadBase);

    request$.subscribe({
      next: () => {
        this.submitting.set(false);
        this.saved.emit();
      },
      error: () => {
        this.submitting.set(false);
        this.submitError.set(this.isEditMode() ? 'Event could not be updated.' : 'Event could not be created.');
      },
    });
  }

  private loadRelatedData(): void {
    this.organizationsService.apiOrganizationsGet().subscribe({
      next: (organizations) => this.organizations.set(Array.isArray(organizations) ? organizations : []),
      error: () => this.organizations.set([]),
    });

    this.contactsService.apiContactsGet().subscribe({
      next: (contacts) => this.contacts.set(Array.isArray(contacts) ? contacts : []),
      error: () => this.contacts.set([]),
    });

    this.locationsService.locationsGet().subscribe({
      next: (locations) => this.locations.set(Array.isArray(locations) ? locations : []),
      error: () => this.locations.set([]),
    });
  }

  protected toggleTargetAudience(audienceCode: number, checked: boolean): void {
    const currentValues = this.form.controls.targetAudiences.value;
    const nextValues = checked
      ? [...currentValues, audienceCode]
      : currentValues.filter((value: number) => value !== audienceCode);

    this.form.controls.targetAudiences.setValue(
      nextValues.sort((left: number, right: number) => left - right),
    );
    this.form.controls.targetAudiences.markAsTouched();
    this.form.controls.targetAudiences.updateValueAndValidity();
  }

  protected isTargetAudienceSelected(audienceCode: number): boolean {
    return this.form.controls.targetAudiences.value.includes(audienceCode);
  }

  protected toggleTopic(topicCode: number, checked: boolean): void {
    const currentValues = this.form.controls.topics.value;
    const nextValues = checked
      ? [...currentValues, topicCode]
      : currentValues.filter((value: number) => value !== topicCode);

    this.form.controls.topics.setValue(nextValues.sort((left: number, right: number) => left - right));
    this.form.controls.topics.markAsTouched();
    this.form.controls.topics.updateValueAndValidity();
  }

  protected isTopicSelected(topicCode: number): boolean {
    return this.form.controls.topics.value.includes(topicCode);
  }

  private trimmedRequiredValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return String(control.value ?? '').trim() ? null : { required: true };
    };
  }

  private requiredNumberArrayValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return Array.isArray(control.value) && control.value.length > 0 ? null : { required: true };
    };
  }

  private dateRangeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const startDate = control.get('startDate')?.value;
      const endDate = control.get('endDate')?.value;

      if (!startDate || !endDate) {
        return null;
      }

      return new Date(endDate) >= new Date(startDate) ? null : { invalidDateRange: true };
    };
  }

  private ageRangeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const ageMinimum = Number(control.get('ageMinimum')?.value);
      const ageMaximum = Number(control.get('ageMaximum')?.value);

      if (Number.isNaN(ageMinimum) || Number.isNaN(ageMaximum)) {
        return null;
      }

      return ageMaximum >= ageMinimum ? null : { invalidAgeRange: true };
    };
  }
}
