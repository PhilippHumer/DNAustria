import { Component, Input, OnInit, output, Signal } from '@angular/core';
import { LocationReplyDto } from '../../api';

@Component({
  selector: 'app-locationcard',
  templateUrl: './locationcard.html',
  styleUrls: ['./locationcard.css']
})
export class LocationCard implements OnInit {

  @Input() location!: LocationReplyDto;

  deleteClicked = output<void>();
  editClicked = output<void>();

  constructor() { }

  ngOnInit() {
  }

  onEditClick(){
    this.editClicked.emit();
  }

  onDeleteClick(){
    this.deleteClicked.emit();
  }

}
