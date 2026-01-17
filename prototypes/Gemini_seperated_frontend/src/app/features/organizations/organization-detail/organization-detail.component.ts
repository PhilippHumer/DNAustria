import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { OrganizationService } from '../../../core/services/organization.service';
import { Organization } from '../../../core/models/organization.model';

@Component({
  selector: 'app-organization-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    RouterModule
  ],
  templateUrl: './organization-detail.component.html',
  styleUrls: ['./organization-detail.component.scss']
})
export class OrganizationDetailComponent implements OnInit {
  form: FormGroup;
  organization: Organization | null = null;
  id: string | null = null;

  constructor(
    private fb: FormBuilder,
    private orgService: OrganizationService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      street: [''],
      city: [''],
      zip: [''],
      state: ['']
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.orgService.getOrganization(this.id).subscribe(org => {
        if (org) {
          this.organization = org;
          this.form.patchValue({
            name: org.name,
            street: org.street || '',
            city: org.city || '',
            zip: org.zip || '',
            state: org.state || ''
          });
        }
      });
    }
  }

  update() {
    if (this.form.valid && this.organization && this.id) {
      const formVal = this.form.value;
      const updatedOrg: Organization = {
        ...this.organization,
        name: formVal.name,
        street: formVal.street,
        city: formVal.city,
        zip: formVal.zip,
        state: formVal.state,
        // Preserve existing if not editable or set defaults
        latitude: this.organization.latitude || 0,
        longitude: this.organization.longitude || 0,
        address: `${formVal.street}, ${formVal.zip} ${formVal.city}`
      };
      
      this.orgService.updateOrganization(this.id, updatedOrg).subscribe(() => {
        this.router.navigate(['/organizations']);
      });
    }
  }

  delete() {
    if (this.id) {
      this.orgService.deleteOrganization(this.id).subscribe(() => {
        this.router.navigate(['/organizations']);
      });
    }
  }
}
