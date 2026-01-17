import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../services/api.service';
import { Organization } from '../../models/organization.model';

@Component({
  selector: 'app-organization-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatSnackBarModule
  ],
  templateUrl: './organization-detail.html',
  styleUrl: './organization-detail.scss'
})
export class OrganizationDetailComponent implements OnInit {
  organizationForm: FormGroup;
  isEditMode = false;
  organizationId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.organizationForm = this.fb.group({
      name: ['', Validators.required],
      address: this.fb.group({
        locationName: [''],
        street: ['', Validators.required],
        zip: ['', Validators.required],
        city: ['', Validators.required],
        country: ['Austria', Validators.required]
      })
    });
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id && id !== 'create') {
        this.isEditMode = true;
        this.organizationId = id;
        this.loadOrganization(this.organizationId);
      }
    });
  }

  loadOrganization(id: string): void {
    this.apiService.getOrganization(id).subscribe(org => {
      this.organizationForm.patchValue(org);
    });
  }

  onSubmit(): void {
    if (this.organizationForm.valid) {
      const organization: Organization = this.organizationForm.value;
      
      if (this.isEditMode && this.organizationId) {
        this.apiService.updateOrganization(this.organizationId, organization).subscribe({
          next: () => {
            this.snackBar.open('Organization updated successfully', 'Close', { duration: 3000 });
            this.router.navigate(['/organizations']);
          },
          error: (err) => {
            console.error('Error updating organization', err);
            this.snackBar.open('Error updating organization', 'Close', { duration: 3000 });
          }
        });
      } else {
        this.apiService.createOrganization(organization).subscribe({
          next: () => {
            this.snackBar.open('Organization created successfully', 'Close', { duration: 3000 });
            this.router.navigate(['/organizations']);
          },
          error: (err) => {
            console.error('Error creating organization', err);
            this.snackBar.open('Error creating organization', 'Close', { duration: 3000 });
          }
        });
      }
    }
  }

  deleteOrganization(): void {
    if (this.isEditMode && this.organizationId) {
      if (confirm('Are you sure you want to delete this organization?')) {
        this.apiService.deleteOrganization(this.organizationId).subscribe({
          next: () => {
            this.snackBar.open('Organization deleted successfully', 'Close', { duration: 3000 });
            this.router.navigate(['/organizations']);
          },
          error: (err) => {
            console.error('Error deleting organization', err);
            this.snackBar.open('Error deleting organization', 'Close', { duration: 3000 });
          }
        });
      }
    }
  }
}
