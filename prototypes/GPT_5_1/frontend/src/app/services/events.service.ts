import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BACKEND_URL, API_KEY } from '../core/api.tokens';

export interface EventDto {
  id?: string;
  title: string;
  description?: string | null;
  topics?: number[] | null;
  dateStart?: string | null;
  dateEnd?: string | null;
  organizationId?: string | null;
  contactId?: string | null;
  targetAudience?: number[] | null;
  isOnline: boolean;
  eventLink?: string | null;
  status?: number;
}

@Injectable({ providedIn: 'root' })
export class EventsService {
  private http = inject(HttpClient);
  private backendUrl = inject(BACKEND_URL);
  private apiKey = inject(API_KEY);
  private base = this.backendUrl + '/server/api/events';
  private headers() { return new HttpHeaders({ 'X-API-Key': this.apiKey }); }

  list(search: string = '', page: number = 1, pageSize: number = 50): Observable<{ total: number; items: EventDto[] }> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    return this.http.get<{ total: number; items: EventDto[] }>(this.base, { params, headers: this.headers() });
  }
  create(dto: EventDto): Observable<EventDto> { return this.http.post<EventDto>(this.base, dto, { headers: this.headers() }); }
  update(id: string, dto: EventDto): Observable<EventDto> { return this.http.put<EventDto>(`${this.base}/${id}`, dto, { headers: this.headers() }); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`, { headers: this.headers() }); }
  import(raw: string): Observable<EventDto> { return this.http.post<EventDto>(`${this.base}/import`, raw, { headers: this.headers() }); }
}
