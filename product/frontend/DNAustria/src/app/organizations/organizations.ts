import {Component, ElementRef, inject, OnInit, signal, ViewChild} from '@angular/core';
import { Organizationcard } from "./organizationcard/organizationcard";
import {FormsModule, NgForm} from '@angular/forms';
import {AddressDto, ContactDto, CreateOrganizationDto, OrganizationDto, OrganizationsService} from '../api';
import { Modal } from 'bootstrap';
import {Contactcard} from '../contacts/contactcard/contactcard';
@Component({
  selector: 'app-organizations',
  imports: [Organizationcard, FormsModule, Contactcard],
  templateUrl: './organizations.html',
  styleUrl: './organizations.css',
})
export class Organizations implements OnInit {

  organizations = signal<OrganizationDto[]>([]);
  organizationService = inject(OrganizationsService);

  protected readonly orgToDelete = signal<OrganizationDto | null>(null);
  protected readonly deleteInProgress = signal(false);
  protected readonly deleteError = signal<string | null>(null);
  protected readonly editMode = signal(false);

  @ViewChild('addOrganizationModal')
  addOrgModal!: ElementRef<HTMLDivElement>;

  @ViewChild('editOrganizationModal')
  editOrgModal!: ElementRef<HTMLDivElement>;

  createOrganization: CreateOrganizationDto = {
    name: '',
    address: {
      street: '',
      zip: '',
      city: '',
      state: '',
    }
  };

  updateOrganization: OrganizationDto = {
    id: 0,
    name: '',
    adress: {
      street: '',
      zip: '',
      city: '',
      state: '',
    }
  };

  addOrg(form: NgForm): void {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }

    this.organizationService.apiOrganizationsPost(
      this.createOrganization).subscribe(
      () => {
        this.loadOrganizations();
        const modalEl = this.addOrgModal.nativeElement;
        const modal = Modal.getInstance(modalEl);

        if (modal) {
          modalEl.addEventListener(
            'hidden.bs.modal',
            () => {
              form.resetForm({
                name: '',
                address: {
                  street: '',
                  zip: '',
                  city: '',
                  state: '',
                }
              });

              document.body.classList.remove('modal-open');
              document.body.style.removeProperty('padding-right');

              document
                .querySelectorAll('.modal-backdrop')
                .forEach(backdrop => backdrop.remove());
            },
            { once: true }
          );

          modal.hide();
        }
      }
    );


    form.resetForm({
      name: '',
      street: '',
      zip: '',
      city: '',
    });
  }

  loadOrganizations(): void {
    this.organizationService.apiOrganizationsGet()
      .subscribe(orgs => this.organizations.set(orgs));
  }

  ngOnInit(): void {
    this.loadOrganizations();
  }

  protected openEditPopup(org: OrganizationDto) {
    this.updateOrganization = org;
    const modalEl = this.editOrgModal.nativeElement;
    if (!modalEl) return;

    const modal = Modal.getOrCreateInstance(modalEl);
    modal.show();
  }

  protected openDeletePopup(org: OrganizationDto) {
    this.orgToDelete.set(org);
    this.deleteError.set(null);
  }

  protected closeDeletePopup(): void {
    this.orgToDelete.set(null);
    this.deleteError.set(null);
  }

  protected confirmDelete(): void {
    const id = this.orgToDelete()?.id;
    if (!id) {
      return;
    }

    this.deleteInProgress.set(true);
    this.deleteError.set(null);

    this.organizationService.apiOrganizationsIdDelete(id).subscribe({
      next: () => {
        this.deleteInProgress.set(false);
        this.orgToDelete.set(null);
        this.loadOrganizations();
      },
      error: () => {
        this.deleteInProgress.set(false);
        this.deleteError.set('Organization could not be deleted.');
      },
    });
  }

  protected updateOrg(form: NgForm) {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }

    this.organizationService.apiOrganizationsIdPut(
      this.updateOrganization.id,
      this.updateOrganization).subscribe(
      () => {
        this.loadOrganizations();
        const modalEl = this.editOrgModal.nativeElement;
        const modal = Modal.getInstance(modalEl);

        if (modal) {
          modalEl.addEventListener(
            'hidden.bs.modal',
            () => {
              form.resetForm({
                name: '',
                address: {
                  street: '',
                  zip: '',
                  city: '',
                  state: '',
                }
              });

              document.body.classList.remove('modal-open');
              document.body.style.removeProperty('padding-right');

              document
                .querySelectorAll('.modal-backdrop')
                .forEach(backdrop => backdrop.remove());
            },
            { once: true }
          );

          modal.hide();
        }
      }
    );


    form.resetForm({
      name: '',
      street: '',
      zip: '',
      city: '',
    });
  }
}
