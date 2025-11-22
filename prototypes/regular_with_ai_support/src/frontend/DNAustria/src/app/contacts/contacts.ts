import { Component } from '@angular/core';
import { Contactcard } from "./contactcard/contactcard";

@Component({
  selector: 'app-contacts',
  imports: [Contactcard],
  templateUrl: './contacts.html',
  styleUrl: './contacts.css',
})
export class Contacts {

}
