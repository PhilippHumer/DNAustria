import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_HOST = ['localhost','127.0.0.1'].includes(window.location.hostname) ? 'http://localhost:5100' : 'http://backend:5100';

export interface ContactDto { id?: string; name: string; email?: string|null; phone?: string|null; organizationId?: string|null; }

@Injectable({ providedIn: 'root' })
export class ContactsService {
  private http = inject(HttpClient);
  private base = API_HOST + '/server/api/contacts';
  private apiKey = 'dev-key';
  private headers() { return new HttpHeaders({ 'X-API-Key': this.apiKey }); }

  list(): Observable<ContactDto[]> { return this.http.get<ContactDto[]>(this.base, { headers: this.headers() }); }
  create(dto: ContactDto): Observable<ContactDto> { return this.http.post<ContactDto>(this.base, dto, { headers: this.headers() }); }
  update(id: string, dto: ContactDto): Observable<ContactDto> { return this.http.put<ContactDto>(`${this.base}/${id}`, dto, { headers: this.headers() }); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`, { headers: this.headers() }); }
}
