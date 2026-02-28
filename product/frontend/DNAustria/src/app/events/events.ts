import { Component } from '@angular/core';
import { Filterevents } from "./filterevents/filterevents";
import { Eventcard } from "./eventcard/eventcard";

@Component({
  selector: 'app-events',
  imports: [Filterevents, Eventcard],
  templateUrl: './events.html',
  styleUrl: './events.css',
})
export class Events {

}
