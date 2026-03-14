import {Component, input, output} from '@angular/core';
import {OrganizationDto} from '../../api';

@Component({
  selector: 'app-organizationcard',
  imports: [],
  templateUrl: './organizationcard.html',
  styleUrl: './organizationcard.css',
})
export class Organizationcard {

  organization = input<OrganizationDto>();
  readonly editClicked = output<void>();
  readonly deleteClicked = output<void>();

  protected onEditClick() {
    this.editClicked.emit();
  }

  protected onDeleteClick() {
    this.deleteClicked.emit();
  }
}
