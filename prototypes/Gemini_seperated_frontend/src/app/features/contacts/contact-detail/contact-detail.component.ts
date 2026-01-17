import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
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
import { Contact } from '../../../core/models/contact.model';

@Component({
  selector: 'app-contact-detail',
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
  templateUrl: './contact-detail.component.html',
  styleUrl: './contact-detail.component.scss'
})
export class ContactDetailComponent implements OnInit {
  form: FormGroup;
  id: string | null = null;
  contact: Contact | undefined;
  organizations$: Observable<Organization[]> | undefined;

  constructor(
    private fb: FormBuilder,
    private service: ContactService,
    private orgService: OrganizationService,
    private route: ActivatedRoute,
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
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.id) {
      this.service.getContact(this.id).subscribe(c => {
        if (c) {
          this.contact = c;
          this.form.patchValue(c);
        } else {
            this.router.navigate(['/contacts']);
        }
      });
    }
  }

  update(): void {
    if (this.form.valid && this.id) {
      this.service.updateContact(this.id, this.form.value).subscribe(() => {
        this.snackBar.open('Contact updated', 'Close', { duration: 2000 });
        this.router.navigate(['/contacts']);
      });
    }
  }

  delete(): void {
    if (this.id && confirm('Are you sure you want to delete this contact?')) {
      this.service.deleteContact(this.id).subscribe(() => {
        this.snackBar.open('Contact deleted', 'Close', { duration: 2000 });
        this.router.navigate(['/contacts']);
      });
    }
  }
}
