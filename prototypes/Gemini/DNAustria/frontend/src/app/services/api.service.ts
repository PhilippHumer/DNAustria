import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Event } from '../models/event.model';
import { Contact } from '../models/contact.model';
import { Organization } from '../models/organization.model';
import { EventStatus, EventClassification } from '../models/enums';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = 'http://localhost:5088/api';

  constructor(private http: HttpClient) { }

  // Events
  getEvents(search?: string, status?: EventStatus): Observable<Event[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (status !== undefined) params = params.set('status', status.toString());
    
    return this.http.get<any>(`${this.apiUrl}/events`, { params }).pipe(
      map(response => {
        // Handle new backend wrapper { events: [...] }
        const rawEvents = response.events || [];
        return rawEvents.map((e: any) => this.mapDtoToEvent(e));
      })
    );
  }

  private mapDtoToEvent(dto: any): Event {
    return {
      id: dto.id || undefined, // Map the ID now that backend provides it
      title: dto.event_title,
      description: dto.event_description,
      eventLink: dto.event_link,
      targetAudience: dto.event_target_audience,
      topics: dto.event_topics,
      dateStart: dto.event_start,
      dateEnd: dto.event_end,
      // Map string back to enum if needed, or keep as string if model allows. 
      // Model expects Enum. Backend sends lowercase string "scheduled".
      classification: this.mapClassification(dto.event_classification),
      fees: dto.event_has_fees,
      isOnline: dto.event_is_online,
      organizationId: '00000000-0000-0000-0000-000000000000', // Missing in DTO
      programName: dto.program_name,
      // ... map other fields
      location: {
         id: '00000000-0000-0000-0000-000000000000',
         locationName: dto.event_location_name,
         street: dto.event_address_street,
         city: dto.event_address_city,
         zip: dto.event_address_zip,
         state: dto.event_address_state,
         latitude: dto.location && dto.location.length > 0 ? dto.location[0] : 0,
         longitude: dto.location && dto.location.length > 1 ? dto.location[1] : 0
      } as any, // casting to avoid strict type checks for partial addr
      contact: {
         id: '00000000-0000-0000-0000-000000000000',
         name: dto.event_contact_name,
         email: dto.event_contact_email,
         phone: dto.event_contact_phone,
         org: dto.event_contact_org
      } as any,
      status: EventStatus.Draft // Defaulting as DTO doesn't seem to have status?
    };
  }

  private mapClassification(val: string): EventClassification {
     if (val === 'scheduled') return EventClassification.Scheduled;
     if (val === 'ondemand') return EventClassification.OnDemand;
     return EventClassification.Scheduled;
  }

  getEvent(id: string): Observable<Event> {
    return this.http.get<Event>(`${this.apiUrl}/events/${id}`);
  }

  createEvent(event: Event): Observable<Event> {
    return this.http.post<Event>(`${this.apiUrl}/events`, event);
  }

  updateEvent(id: string, event: Event): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/events/${id}`, event);
  }

  deleteEvent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/events/${id}`);
  }

  importEvent(text: string): Observable<Event> {
    return this.http.post<Event>(`${this.apiUrl}/import/text`, { text });
  }

  // Contacts
  getContacts(): Observable<Contact[]> {
    return this.http.get<Contact[]>(`${this.apiUrl}/contacts`);
  }

  getContact(id: string): Observable<Contact> {
    return this.http.get<Contact>(`${this.apiUrl}/contacts/${id}`);
  }

  createContact(contact: Contact): Observable<Contact> {
    return this.http.post<Contact>(`${this.apiUrl}/contacts`, contact);
  }

  updateContact(id: string, contact: Contact): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/contacts/${id}`, contact);
  }

  deleteContact(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/contacts/${id}`);
  }

  // Organizations
  getOrganizations(): Observable<Organization[]> {
    return this.http.get<Organization[]>(`${this.apiUrl}/organizations`);
  }

  getOrganization(id: string): Observable<Organization> {
    return this.http.get<Organization>(`${this.apiUrl}/organizations/${id}`);
  }

  createOrganization(organization: Organization): Observable<Organization> {
    return this.http.post<Organization>(`${this.apiUrl}/organizations`, organization);
  }

  updateOrganization(id: string, organization: Organization): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/organizations/${id}`, organization);
  }

  deleteOrganization(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/organizations/${id}`);
  }
}
