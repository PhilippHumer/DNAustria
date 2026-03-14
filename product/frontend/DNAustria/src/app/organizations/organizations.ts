import {Component, ElementRef, inject, OnInit, signal, ViewChild} from '@angular/core';
import { Organizationcard } from "./organizationcard/organizationcard";
import {FormsModule, NgForm} from '@angular/forms';
import {AddressDto, CreateOrganizationDto, OrganizationDto, OrganizationsService} from '../api';
import { Modal } from 'bootstrap';
@Component({
  selector: 'app-organizations',
  imports: [Organizationcard, FormsModule],
  templateUrl: './organizations.html',
  styleUrl: './organizations.css',
})
export class Organizations implements OnInit {

  organizations = signal<OrganizationDto[]>([]);
  organizationService = inject(OrganizationsService);

  @ViewChild('addOrganizationModal')
  modalElement!: ElementRef<HTMLDivElement>;

  organization: CreateOrganizationDto = {
    name: '',
    address: {
      street: '',
      zip: '',
      city: '',
      state: '',
    }
  };

  onSubmit(form: NgForm): void {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }

    console.log('Organization submitted:', this.organization);

    this.organizationService.apiOrganizationsPost(
      this.organization).subscribe(
      () => {
        this.loadOrganizations();



        const modalEl = this.modalElement.nativeElement;
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
}
