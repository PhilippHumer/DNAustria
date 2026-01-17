import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { OrganizationService } from '../../../core/services/organization.service';
import { Organization } from '../../../core/models/organization.model';

@Component({
  selector: 'app-organization-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatButtonModule, MatIconModule],
  templateUrl: './organization-list.component.html',
  styleUrl: './organization-list.component.scss'
})
export class OrganizationListComponent implements OnInit {
  organizations: Organization[] = [];
  displayedColumns: string[] = ['name', 'address', 'actions'];

  constructor(private orgService: OrganizationService) {}

  ngOnInit(): void {
    this.orgService.getOrganizations().subscribe(data => this.organizations = data);
  }

  deleteOrg(id: string, event: Event): void {
    event.stopPropagation();
    if (confirm('Delete organization?')) {
        this.orgService.deleteOrganization(id).subscribe(() => {
            this.orgService.getOrganizations().subscribe(data => this.organizations = data);
        });
    }
  }
}
