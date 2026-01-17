import { Component, signal } from '@angular/core';
import { ContactsService, ContactDto } from '../services/contacts.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector:'app-contacts-list',
  standalone:true,
  imports:[CommonModule, FormsModule, ReactiveFormsModule, MatTableModule, MatButtonModule],
  template:`<h2>Contacts</h2>
  <form [formGroup]="form" (ngSubmit)="save()" class="form">
    <input formControlName="name" placeholder="Name" required />
    <input formControlName="email" placeholder="Email" />
    <input formControlName="phone" placeholder="Phone" />
    <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">{{form.value.id? 'Update':'Add'}}</button>
    <button mat-button type="button" (click)="reset()">Reset</button>
  </form>
  <table mat-table [dataSource]="items()">
    <ng-container matColumnDef="name"><th mat-header-cell *matHeaderCellDef>Name</th><td mat-cell *matCellDef="let c">{{c.name}}</td></ng-container>
    <ng-container matColumnDef="email"><th mat-header-cell *matHeaderCellDef>Email</th><td mat-cell *matCellDef="let c">{{c.email}}</td></ng-container>
    <ng-container matColumnDef="actions"><th mat-header-cell *matHeaderCellDef></th><td mat-cell *matCellDef="let c"><button mat-button (click)="edit(c)">Edit</button><button mat-button color="warn" (click)="remove(c)">Del</button></td></ng-container>
    <tr mat-header-row *matHeaderRowDef="cols"></tr>
    <tr mat-row *matRowDef="let row; columns: cols;"></tr>
  </table>`,
  styles:[`.form{display:flex;gap:.5rem;flex-wrap:wrap;margin-bottom:1rem}`]
})
export class ContactsListComponent {
  items = signal<ContactDto[]>([]);
  cols = ['name','email','actions'];
  form: FormGroup;
  constructor(private fb: FormBuilder, private service: ContactsService){
    this.form = this.fb.group({
      id: [null],
      name: ['', Validators.required],
      email: [null],
      phone: [null]
    });
  }
  ngOnInit(){ this.load(); }
  load(){ this.service.list().subscribe((r: ContactDto[])=> this.items.set(r)); }
  save(){ const v = this.form.getRawValue(); const dto: ContactDto = { ...v } as ContactDto; const obs = dto.id? this.service.update(dto.id!,dto): this.service.create(dto); obs.subscribe(()=>{this.load(); this.reset();}); }
  edit(c: ContactDto){ this.form.patchValue({ ...c }); }
  remove(c: ContactDto){ if(!confirm('Delete?')) return; this.service.delete(c.id!).subscribe(()=> this.load()); }
  reset(){ this.form.reset({ id: null, name: '', email: null, phone: null }); }
}
