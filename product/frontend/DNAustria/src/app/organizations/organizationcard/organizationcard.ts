import {Component, input} from '@angular/core';
import {OrganizationDto} from '../../api';

@Component({
  selector: 'app-organizationcard',
  imports: [],
  templateUrl: './organizationcard.html',
  styleUrl: './organizationcard.css',
})
export class Organizationcard {

  organization = input<OrganizationDto>();
}
