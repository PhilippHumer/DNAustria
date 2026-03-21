import { Component, OnInit, signal } from '@angular/core';
import { LocationCard } from './locationcard/locationcard';
import { LocationReplyDto, LocationsService } from '../api';
import { CreateLocationComponent } from './create-location/create-location';
@Component({
  selector: 'app-locations',
  imports:[
    LocationCard,
    CreateLocationComponent
  ],
  templateUrl: './locations.html',
  styleUrls: ['./locations.css']
})
export class Locations implements OnInit {
  locations = signal<LocationReplyDto[]>([])

  isCreatePopupOpen = signal<boolean>(false);
  locationToDelete = signal<LocationReplyDto|null>(null)
  deleteError = signal<string>("")
  editingLocation = signal<LocationReplyDto|null>(null)
  deleteInProgress = signal<boolean>(false)

  constructor(private service: LocationsService) { }

  ngOnInit() {
    this.loadLocations();
  }

  loadLocations(){
    this.service.locationsGet().subscribe(x => this.locations.set(x));
  }

  openPopupMenu(){
    this.isCreatePopupOpen.set(true);
    this.editingLocation.set(null);
  }

  openEditPopup(location:LocationReplyDto){
    this.editingLocation.set(location);
    this.isCreatePopupOpen.set(true);
  }

  closePopupMenu(){
    this.isCreatePopupOpen.set(false);
    this.editingLocation.set(null);
  }

  onLocationCreated(){
    this.isCreatePopupOpen.set(false);
    this.loadLocations();
  }

  handleLocationSave(){
    this.isCreatePopupOpen.set(false);
    this.editingLocation.set(null);
    this.loadLocations();
  }

  confirmDelete(){
    const id = this.locationToDelete()?.id;
    if (!id) {
      return;
    }

    this.deleteInProgress.set(true);
    this.deleteError.set("");
    this.deleteInProgress.set(true);
    this.service.locationsIdDelete(id).subscribe({
      next: () => {
        this.deleteInProgress.set(false);
        this.locationToDelete.set(null);
        this.service.locationsGet().subscribe(x => this.locations.set(x));
      },
      error: () => {
        this.deleteInProgress.set(false);
        this.deleteError.set('Contact could not be deleted.');
      },
    });
  }

  closeDeletePopup(){
      this.locationToDelete.set(null);
      this.deleteInProgress.set(false);
  }

    protected openDeletePopup(contact: LocationReplyDto): void {
      this.locationToDelete.set(contact);
      this.deleteError.set("");
    }
}
