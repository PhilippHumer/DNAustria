import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Organization } from '../models/organization.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  private apiUrl = `${environment.apiUrl}/organization`;

  constructor(private http: HttpClient) {}

  getOrganizations(): Observable<Organization[]> {
    return this.http.get<Organization[]>(this.apiUrl);
  }

  getOrganization(id: string): Observable<Organization> {
    return this.http.get<Organization>(`${this.apiUrl}/${id}`);
  }

  createOrganization(org: Partial<Organization>): Observable<Organization> {
    return this.http.post<Organization>(this.apiUrl, org);
  }

  updateOrganization(id: string, org: Partial<Organization>): Observable<Organization> {
    return this.http.put<Organization>(`${this.apiUrl}/${id}`, org);
  }

  deleteOrganization(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
