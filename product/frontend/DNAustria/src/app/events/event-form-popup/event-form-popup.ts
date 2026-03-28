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
import { environment } from '../../environment';

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
  readonly llmPrefillLoading = signal(false);
  readonly llmPrefillError = signal<string | null>(null);
  readonly llmPrefillInfo = signal<string | null>(null);
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

  protected async analyzeAndPrefill(prompt: string): Promise<void> {
    const trimmedPrompt = prompt.trim();
    if (!trimmedPrompt) {
      this.llmPrefillError.set('Please enter text before running the analysis.');
      this.llmPrefillInfo.set(null);
      return;
    }

    this.llmPrefillLoading.set(true);
    this.llmPrefillError.set(null);
    this.llmPrefillInfo.set(null);

    const apiBaseUrl = environment.apiUrl.replace(/\/$/, '');

    try {
      const response = await fetch(`${apiBaseUrl}/api/events/llm`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ prompt: trimmedPrompt }),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || `HTTP ${response.status}`);
      }

      const contentType = response.headers.get('content-type') || '';
      const responseBody: unknown = contentType.includes('application/json')
        ? await response.json()
        : await response.text();

      const prefillData = this.extractPrefillPayload(responseBody);
      if (!prefillData) {
        throw new Error('The LLM response did not contain a valid JSON object.');
      }

      const updatedFieldCount = this.applyLlmPrefill(prefillData);
      if (updatedFieldCount === 0) {
        this.llmPrefillInfo.set('No matching event fields were found in the LLM response.');
      } else {
        this.llmPrefillInfo.set(`Prefilled ${updatedFieldCount} field${updatedFieldCount === 1 ? '' : 's'} from LLM output.`);
      }
    } catch (error) {
      console.error('LLM prefill failed', error);
      const message = error instanceof Error ? error.message : String(error);
      this.llmPrefillError.set(`Could not prefill event fields: ${message}`);
    } finally {
      this.llmPrefillLoading.set(false);
    }
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

  private applyLlmPrefill(data: Record<string, unknown>): number {
    const patch: Partial<{
      name: string;
      description: string;
      link: string;
      startDate: string;
      endDate: string;
      classification: number;
      status: number;
      hasFees: boolean;
      isOnline: boolean;
      organization: string;
      programName: string;
      format: string;
      schoolBookable: boolean;
      ageMinimum: number;
      ageMaximum: number;
      location: string;
      contact: string;
      targetAudiences: number[];
      topics: number[];
    }> = {};

    this.setStringIfPresent(patch, 'name', data['name']);
    this.setStringIfPresent(patch, 'description', data['description']);
    this.setStringIfPresent(patch, 'link', data['link']);
    this.setStringIfPresent(patch, 'programName', data['programName']);
    this.setStringIfPresent(patch, 'format', data['format']);

    const startDate = this.toDatetimeLocalString(data['startDate']);
    if (startDate) {
      patch.startDate = startDate;
    }

    const endDate = this.toDatetimeLocalString(data['endDate']);
    if (endDate) {
      patch.endDate = endDate;
    }

    const classification = this.toNumber(data['classification']);
    if (classification !== null) {
      patch.classification = classification;
    }

    const status = this.toNumber(data['status']);
    if (status !== null) {
      patch.status = status;
    }

    const hasFees = this.toBoolean(data['hasFees']);
    if (hasFees !== null) {
      patch.hasFees = hasFees;
    }

    const isOnline = this.toBoolean(data['isOnline']);
    if (isOnline !== null) {
      patch.isOnline = isOnline;
    }

    const schoolBookable = this.toBoolean(data['schoolBookable']);
    if (schoolBookable !== null) {
      patch.schoolBookable = schoolBookable;
    }

    const ageMinimum = this.toNumber(data['ageMinimum']);
    if (ageMinimum !== null) {
      patch.ageMinimum = ageMinimum;
    }

    const ageMaximum = this.toNumber(data['ageMaximum']);
    if (ageMaximum !== null) {
      patch.ageMaximum = ageMaximum;
    }

    const targetAudiences = this.toNumberArray(data['targetAudiences']);
    if (targetAudiences) {
      const allowedTargetAudiences = new Set(EVENT_TARGET_AUDIENCE_OPTIONS.map((option) => option.value));
      patch.targetAudiences = targetAudiences.filter((value) => allowedTargetAudiences.has(value));
    }

    const topics = this.toNumberArray(data['topics']);
    if (topics) {
      const allowedTopics = new Set(EVENT_TOPIC_OPTIONS.map((option) => option.value));
      patch.topics = topics.filter((value) => allowedTopics.has(value));
    }

    const organizationId = this.resolveEntityId(
      data['organization'] ?? data['organizationId'] ?? data['organizationName'],
      this.organizations(),
      (organization) => organization.id,
      (organization) => organization.name,
    );
    if (organizationId !== null) {
      patch.organization = String(organizationId);
    }

    const locationId = this.resolveEntityId(
      data['location'] ?? data['locationId'] ?? data['locationName'],
      this.locations(),
      (location) => location.id,
      (location) => location.name,
    );
    if (locationId !== null) {
      patch.location = String(locationId);
    }

    const contactId = this.resolveEntityId(
      data['contact'] ?? data['contactId'] ?? data['contactName'],
      this.contacts(),
      (contact) => contact.id,
      (contact) => contact.name,
    );
    if (contactId !== null) {
      patch.contact = String(contactId);
    }

    this.form.patchValue(patch);
    this.form.markAsDirty();
    this.form.updateValueAndValidity();

    return Object.keys(patch).length;
  }

  private extractPrefillPayload(body: unknown): Record<string, unknown> | null {
    if (this.isRecord(body)) {
      if (this.isRecord(body['event'])) {
        return body['event'];
      }

      if (this.isRecord(body['data'])) {
        return body['data'];
      }

      return body;
    }

    if (typeof body !== 'string') {
      return null;
    }

    const candidates = [body.trim()]
      .concat(this.extractCodeBlock(body))
      .concat(this.extractJsonObject(body))
      .filter((candidate) => candidate.length > 0);

    for (const candidate of candidates) {
      try {
        const parsed = JSON.parse(candidate) as unknown;
        if (this.isRecord(parsed)) {
          if (this.isRecord(parsed['event'])) {
            return parsed['event'];
          }

          if (this.isRecord(parsed['data'])) {
            return parsed['data'];
          }

          return parsed;
        }
      } catch {
        // Ignore malformed candidate snippets and continue trying others.
      }
    }

    return null;
  }

  private extractCodeBlock(value: string): string {
    const codeBlockMatch = value.match(/```(?:json)?\s*([\s\S]*?)```/i);
    return codeBlockMatch?.[1]?.trim() ?? '';
  }

  private extractJsonObject(value: string): string {
    const firstBraceIndex = value.indexOf('{');
    const lastBraceIndex = value.lastIndexOf('}');
    if (firstBraceIndex < 0 || lastBraceIndex < 0 || lastBraceIndex <= firstBraceIndex) {
      return '';
    }

    return value.slice(firstBraceIndex, lastBraceIndex + 1).trim();
  }

  private isRecord(value: unknown): value is Record<string, unknown> {
    return typeof value === 'object' && value !== null && !Array.isArray(value);
  }

  private setStringIfPresent<T extends Record<string, unknown>>(target: T, key: keyof T, value: unknown): void {
    if (typeof value !== 'string') {
      return;
    }

    const trimmed = value.trim();
    if (!trimmed) {
      return;
    }

    target[key] = trimmed as T[keyof T];
  }

  private toDatetimeLocalString(value: unknown): string | null {
    if (typeof value !== 'string' || !value.trim()) {
      return null;
    }

    const datetimeLocal = toDatetimeLocalValue(value);
    return datetimeLocal || null;
  }

  private toNumber(value: unknown): number | null {
    if (typeof value === 'number' && Number.isFinite(value)) {
      return value;
    }

    if (typeof value === 'string' && value.trim()) {
      const numberValue = Number(value);
      return Number.isFinite(numberValue) ? numberValue : null;
    }

    return null;
  }

  private toBoolean(value: unknown): boolean | null {
    if (typeof value === 'boolean') {
      return value;
    }

    if (typeof value === 'string') {
      const normalized = value.trim().toLowerCase();
      if (normalized === 'true') {
        return true;
      }

      if (normalized === 'false') {
        return false;
      }
    }

    return null;
  }

  private toNumberArray(value: unknown): number[] | null {
    if (Array.isArray(value)) {
      const parsedArray = value
        .map((entry) => this.toNumber(entry))
        .filter((entry): entry is number => entry !== null);

      return [...new Set(parsedArray)].sort((left, right) => left - right);
    }

    if (typeof value === 'string' && value.trim()) {
      const parsedArray = value
        .split(',')
        .map((entry) => this.toNumber(entry))
        .filter((entry): entry is number => entry !== null);

      return [...new Set(parsedArray)].sort((left, right) => left - right);
    }

    return null;
  }

  private resolveEntityId<T>(
    value: unknown,
    entries: T[],
    getId: (entry: T) => number | undefined,
    getName: (entry: T) => string | undefined,
  ): number | null {
    const directId = this.toNumber(value);
    if (directId !== null) {
      return directId;
    }

    if (typeof value !== 'string') {
      return null;
    }

    const normalizedValue = value.trim().toLowerCase();
    if (!normalizedValue) {
      return null;
    }

    const matchedEntry = entries.find((entry) => getName(entry)?.trim().toLowerCase() === normalizedValue);
    const matchedId = matchedEntry ? getId(matchedEntry) : undefined;

    return typeof matchedId === 'number' ? matchedId : null;
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
