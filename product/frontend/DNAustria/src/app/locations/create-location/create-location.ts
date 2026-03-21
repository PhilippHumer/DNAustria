import { Component, computed, effect, input, OnInit, output, signal } from '@angular/core';
import * as L from 'leaflet';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { combineLatest } from 'rxjs';
import { debounceTime, startWith } from 'rxjs/operators';
import { LocationReplyDto, LocationsService } from '../../api';

@Component({
  selector: 'app-create-location',
  imports: [ReactiveFormsModule],
  templateUrl: './create-location.html',
  styleUrls: ['./create-location.css']
})
export class CreateLocationComponent implements OnInit {

editLocation = input<LocationReplyDto|null>(null);
isEditMode = computed(() => this.editLocation() != null);

readonly submitting = signal(false);
readonly submitError = signal<string | null>(null);

readonly cancel = output<void>();
readonly saved = output<void>();

addLocationForm = new FormGroup({
    'name': new FormControl<string>("", [Validators.required]),
    'latitude': new FormControl<number>(48.3687, [Validators.min(-90), Validators.max(90)]),
    'longitude': new FormControl<number>(14.5166, [Validators.min(-180), Validators.max(180)]),
    'street': new FormControl<string>("Softwarepark 11", [Validators.required]),
    'zip': new FormControl<number>(4232, [Validators.required]),
    'city': new FormControl<string>("Hagenberg", [Validators.required]),
    'state': new FormControl<string>("Oberösterreich", [Validators.required])
  })

  marker:any;
  map!:L.Map;

  constructor(private service: LocationsService) {
    effect(() => {
      const location = this.editLocation();
      if(location){
         this.addLocationForm.setValue({
          name: location.name ?? '',
          latitude: location.latitude ?? 0 ,
          longitude: location.longitude ?? 0,
          street: location.address?.street ?? '',
          zip: parseFloat(location.address!.zip ?? "0"),
          city: location.address?.city ?? "",
          state: location.address?.state ?? ""
        });
        return;
      }
      this.addLocationForm.reset({
          name: "",
          latitude: 0,
          longitude: 0,
          street: "",
          zip: 1000,
          city: "",
          state: ""
      });
    });
  }

  ngOnInit() {
    const latControl = this.addLocationForm.get('latitude')!;
    const lngControl = this.addLocationForm.get('longitude')!;

    const lat$ = latControl.valueChanges.pipe(startWith(latControl.value));
    const lng$ = lngControl.valueChanges.pipe(startWith(lngControl.value));

    combineLatest([lat$, lng$])
      .pipe(debounceTime(500))
      .subscribe(([lat, lng]) => {
        if (lat && lng) {
          this.onLatLngChange(lat, lng);
        }
      });
  }

  ngAfterViewInit() {
    delete (L.Icon.Default.prototype as any)._getIconUrl;

    L.Icon.Default.mergeOptions({
      iconRetinaUrl: 'assets/leaflet/marker-icon-2x.png',
      iconUrl: 'assets/leaflet/marker-icon.png',
      shadowUrl: 'assets/leaflet/marker-shadow.png',
    });

    this.map = L.map('map').setView([48.3687, 14.5166], 13); //default: Hagenberg because we can
    this.marker = L.marker([48.3687, 14.5166]).addTo(this.map)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap'
    }).addTo(this.map);

    this.map.on('click', (e: any) => {
      const {lat, lng} = e.latlng;
      if(this.marker){
        this.marker.setLatLng([lat, lng]);
      }
      else{
        this.marker = L.marker([lat, lng]).addTo(this.map)
      }
      this.addLocationForm.get("latitude")?.setValue(lat)
      this.addLocationForm.get("longitude")?.setValue(lng)
    });
  }

  onLatLngChange(lat:number, lng:number){
    if(this.marker){
        this.marker.setLatLng([lat, lng]);
      }
      else{
        this.marker = L.marker([lat, lng]).addTo(this.map)
      }
      this.map.setView([lat, lng], 13)
  }

  createLocation(){
    if(!this.addLocationForm.valid){
      this.addLocationForm.markAllAsTouched();
      return;
    }
    var id = this.editLocation()?.id;
      this.submitError.set(null);
      this.submitting.set(true);
      var latEmpty = this.addLocationForm.get("latitude")?.value?.toString() == "";
      var lngEmpty = this.addLocationForm.get("longitude")?.value?.toString() == "";
      if(id){
        this.service.locationsIdPut(id, {
          name: this.addLocationForm.get("name")!.value!,
          latitude: latEmpty ? 0 : this.addLocationForm.get("latitude")!.value ?? 0,
          longitude: lngEmpty ? 0 : this.addLocationForm.get("longitude")!.value ?? 0,
          address: {
            zip: this.addLocationForm.get("zip")!.value!.toString(),
            city: this.addLocationForm.get("city")!.value!,
            state: this.addLocationForm.get("state")!.value!,
            street: this.addLocationForm.get("street")!.value!
          }}).subscribe({
            next: () => {
              this.submitting.set(false);
              this.saved.emit();
            },
            error: () => {
              this.submitting.set(false);
              this.submitError.set(this.editLocation() ? "Location could not be updated" : "Location could not be created")
            }
          })
      }
      else{
        this.service.locationsPost({
          name: this.addLocationForm.get("name")!.value!,
          latitude: latEmpty ? 0 : this.addLocationForm.get("latitude")?.value ?? 0,
          longitude: lngEmpty ? 0 : this.addLocationForm.get("longitude")?.value ?? 0,
          address: {
            zip: this.addLocationForm.get("zip")!.value!.toString(),
            city: this.addLocationForm.get("city")!.value!,
            state: this.addLocationForm.get("state")!.value!,
            street: this.addLocationForm.get("street")!.value!
          }}).subscribe({
            next: () => {
              this.submitting.set(false);
              this.saved.emit();
            },
            error: () => {
              this.submitting.set(false);
              this.submitError.set(this.editLocation() ? "Location could not be updated" : "Location could not be created")
            }
          })
      }
  }

  cancelForm(){
    this.cancel.emit();
  }
}
