import { Component, signal } from '@angular/core';
import { OrganizationsService, OrganizationDto } from '../services/organizations.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector:'app-organizations-list',
  standalone:true,
  imports:[CommonModule, FormsModule, ReactiveFormsModule, MatTableModule, MatButtonModule],
  template:`<h2>Organizations</h2>
  <form [formGroup]="form" (ngSubmit)="save()" class="form">
    <input formControlName="name" placeholder="Name" required />
    <input formControlName="addressStreet" placeholder="Street" />
    <input formControlName="addressCity" placeholder="City" />
    <input formControlName="addressZip" placeholder="ZIP" />
    <input formControlName="regionId" placeholder="Region" type="number" />
    <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">{{form.value.id? 'Update':'Add'}}</button>
    <button mat-button type="button" (click)="reset()">Reset</button>
  </form>
  <table mat-table [dataSource]="items()">
    <ng-container matColumnDef="name"><th mat-header-cell *matHeaderCellDef>Name</th><td mat-cell *matCellDef="let o">{{o.name}}</td></ng-container>
    <ng-container matColumnDef="city"><th mat-header-cell *matHeaderCellDef>City</th><td mat-cell *matCellDef="let o">{{o.addressCity}}</td></ng-container>
    <ng-container matColumnDef="actions"><th mat-header-cell *matHeaderCellDef></th><td mat-cell *matCellDef="let o"><button mat-button (click)="edit(o)">Edit</button><button mat-button color="warn" (click)="remove(o)">Del</button></td></ng-container>
    <tr mat-header-row *matHeaderRowDef="cols"></tr>
    <tr mat-row *matRowDef="let row; columns: cols;"></tr>
  </table>`,
  styles:[`.form{display:flex;gap:.5rem;flex-wrap:wrap;margin-bottom:1rem}`]
})
export class OrganizationsListComponent {
  items = signal<OrganizationDto[]>([]);
  cols = ['name','city','actions'];
  form: FormGroup;
  constructor(private fb: FormBuilder, private service: OrganizationsService){
    this.form = this.fb.group({
      id: [null],
      name: ['', Validators.required],
      addressStreet: [null],
      addressCity: [null],
      addressZip: [null],
      regionId: [null]
    });
  }
  ngOnInit(){ this.load(); }
  load(){ this.service.list().subscribe(r => this.items.set(r)); }
  save(){ const v = this.form.getRawValue(); const dto: OrganizationDto = { ...v } as OrganizationDto; const obs = dto.id? this.service.update(dto.id,dto): this.service.create(dto); obs.subscribe(()=>{this.load(); this.reset();}); }
  edit(o: OrganizationDto){ this.form.patchValue({ ...o }); }
  remove(o: OrganizationDto){ if(!confirm('Delete?')) return; this.service.delete(o.id!).subscribe(()=> this.load()); }
  reset(){ this.form.reset({ id: null, name: '', addressStreet: null, addressCity: null, addressZip: null, regionId: null }); }
}
