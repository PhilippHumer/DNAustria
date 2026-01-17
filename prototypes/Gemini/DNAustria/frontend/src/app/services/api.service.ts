import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event } from '../models/event.model';
import { Contact } from '../models/contact.model';
import { Organization } from '../models/organization.model';
import { EventStatus } from '../models/enums';

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
    return this.http.get<Event[]>(`${this.apiUrl}/events`, { params });
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
