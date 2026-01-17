import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../services/api.service';
import { Organization } from '../../models/organization.model';

import { Observable } from 'rxjs';

@Component({
  selector: 'app-organization-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './organization-list.html',
  styleUrl: './organization-list.scss'
})
export class OrganizationListComponent implements OnInit {
  organizations$!: Observable<Organization[]>;
  displayedColumns: string[] = ['name', 'address'];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.organizations$ = this.apiService.getOrganizations();
  }
}
