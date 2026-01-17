import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ApiService } from '../../services/api.service';
import { Contact } from '../../models/contact.model';

@Component({
  selector: 'app-contact-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    RouterModule
  ],
  templateUrl: './contact-detail.html',
  styleUrl: './contact-detail.scss'
})
export class ContactDetailComponent implements OnInit {
  form: FormGroup;
  contactId?: string;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.email]],
      phone: [''],
      org: ['']
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'create') {
      this.contactId = id;
      this.isEditMode = true;
      this.loadContact(this.contactId);
    }
  }

  loadContact(id: string) {
    this.apiService.getContact(id).subscribe(contact => {
      this.form.patchValue(contact);
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const contact: Contact = { ...this.form.value, id: this.contactId };
      
      if (this.isEditMode && this.contactId) {
        this.apiService.updateContact(this.contactId, contact).subscribe(() => {
          this.snackBar.open('Contact updated', 'Close', { duration: 3000 });
          this.router.navigate(['/contacts']);
        });
      } else {
        this.apiService.createContact(contact).subscribe(() => {
          this.snackBar.open('Contact created', 'Close', { duration: 3000 });
          this.router.navigate(['/contacts']);
        });
      }
    }
  }

  onDelete() {
    if (this.contactId && confirm('Are you sure you want to delete this contact?')) {
      this.apiService.deleteContact(this.contactId).subscribe(() => {
        this.snackBar.open('Contact deleted', 'Close', { duration: 3000 });
        this.router.navigate(['/contacts']);
      });
    }
  }
}
