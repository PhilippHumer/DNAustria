import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_HOST = ['localhost','127.0.0.1'].includes(window.location.hostname) ? 'http://localhost:5100' : 'http://backend:5100';

export interface OrganizationDto { id?: string; name: string; addressStreet?: string|null; addressCity?: string|null; addressZip?: string|null; regionId?: number|null; }

@Injectable({providedIn:'root'})
export class OrganizationsService {
  private http = inject(HttpClient);
  private base = API_HOST + '/server/api/organizations';
  list(): Observable<OrganizationDto[]> { return this.http.get<OrganizationDto[]>(this.base); }
  create(dto: OrganizationDto): Observable<OrganizationDto> { return this.http.post<OrganizationDto>(this.base,dto); }
  update(id:string,dto:OrganizationDto): Observable<OrganizationDto> { return this.http.put<OrganizationDto>(`${this.base}/${id}`,dto); }
  delete(id:string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
