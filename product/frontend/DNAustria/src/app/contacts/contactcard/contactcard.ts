import { Component, input, output } from '@angular/core';
import { ContactDto } from '../../api/model/contactDto';

@Component({
  selector: 'app-contactcard',
  imports: [],
  templateUrl: './contactcard.html',
  styleUrl: './contactcard.css'
})
export class Contactcard {
  readonly contact = input.required<ContactDto>();
  readonly editClicked = output<void>();
  readonly deleteClicked = output<void>();

  protected onEditClick(): void {
    this.editClicked.emit();
  }

  protected onDeleteClick(): void {
    this.deleteClicked.emit();
  }
}
