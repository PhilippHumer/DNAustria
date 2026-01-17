import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { Observable } from 'rxjs';

import { ContactService } from '../../../core/services/contact.service';
import { OrganizationService } from '../../../core/services/organization.service';
import { Organization } from '../../../core/models/organization.model';

@Component({
  selector: 'app-contact-create',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    RouterModule, 
    MatButtonModule, 
    MatInputModule, 
    MatSelectModule,
    MatOptionModule,
    MatFormFieldModule, 
    MatSnackBarModule
  ],
  templateUrl: './contact-create.component.html',
  styleUrl: './contact-create.component.scss'
})
export class ContactCreateComponent implements OnInit {
  form: FormGroup;
  organizations$: Observable<Organization[]> | undefined;

  constructor(
    private fb: FormBuilder, 
    private service: ContactService, 
    private orgService: OrganizationService,
    private router: Router, 
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      org: ['']
    });
  }

  ngOnInit(): void {
    this.organizations$ = this.orgService.getOrganizations();
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.service.createContact(this.form.value).subscribe(() => {
        this.snackBar.open('Contact created!', 'Close', { duration: 2000 });
        this.router.navigate(['/contacts']);
      });
    }
  }
}
