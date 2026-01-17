import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Router, RouterModule } from '@angular/router';
import { OrganizationService } from '../../../core/services/organization.service';
import { Organization } from '../../../core/models/organization.model';

@Component({
  selector: 'app-organization-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    RouterModule
  ],
  templateUrl: './organization-create.component.html',
  styleUrls: ['./organization-create.component.scss']
})
export class OrganizationCreateComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private orgService: OrganizationService,
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

  save() {
    if (this.form.valid) {
      const formVal = this.form.value;
      const newOrg: Organization = {
        id: crypto.randomUUID(), 
        name: formVal.name,
        street: formVal.street,
        city: formVal.city,
        zip: formVal.zip,
        state: formVal.state,
        latitude: 0,
        longitude: 0,
        address: `${formVal.street}, ${formVal.zip} ${formVal.city}` // rudimentary formatting
      };
      
      this.orgService.createOrganization(newOrg).subscribe(() => {
        this.router.navigate(['/organizations']);
      });
    }
  }
}
