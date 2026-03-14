import { Component, OnInit } from '@angular/core';
import { LocationCard } from './locationcard/locationcard';

@Component({
  selector: 'app-locations',
  imports:[
    LocationCard
  ],
  templateUrl: './locations.html',
  styleUrls: ['./locations.css']
})
export class Locations implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
