import { Component } from '@angular/core';
import { Organizationcard } from "./organizationcard/organizationcard";

@Component({
  selector: 'app-organizations',
  imports: [Organizationcard],
  templateUrl: './organizations.html',
  styleUrl: './organizations.css',
})
export class Organizations {

}
