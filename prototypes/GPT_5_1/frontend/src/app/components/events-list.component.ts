import { Component, inject, signal } from '@angular/core';
import { EventsService, EventDto } from '../services/events.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { EventFormComponent } from './event-form.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-events-list',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTableModule, MatButtonModule, MatDialogModule, MatIconModule, MatFormFieldModule, MatInputModule, MatSelectModule, EventFormComponent],
  template: `
  <div class="toolbar">
    <button mat-raised-button color="primary" (click)="openForm()">Neu</button>
    <mat-form-field appearance="outline" style="width:200px">
      <mat-label>Suche Titel</mat-label>
      <input matInput [(ngModel)]="search" (input)="debouncedLoad()" placeholder="z.B. AI" />
    </mat-form-field>
    <mat-form-field appearance="outline" style="width:150px">
      <mat-label>Status</mat-label>
      <mat-select [(ngModel)]="statusFilter" (selectionChange)="load()">
        <mat-option [value]="''">Alle</mat-option>
        <mat-option [value]="0">Draft</mat-option>
        <mat-option [value]="1">Approved</mat-option>
        <mat-option [value]="2">Transferred</mat-option>
      </mat-select>
    </mat-form-field>
    <span class="flex-spacer"></span>
    <button mat-button (click)="prev()" [disabled]="page===1">Prev</button>
    <span>Seite {{page}}</span>
    <button mat-button (click)="next()" [disabled]="page*pageSize >= total">Next</button>
  </div>
  <table mat-table [dataSource]="items()" class="mat-elevation-z1 full-width">
    <!-- existing columns -->
    <ng-container matColumnDef="title">
      <th mat-header-cell *matHeaderCellDef>Titel</th>
      <td mat-cell *matCellDef="let e">{{e.title}}</td>
    </ng-container>
    <ng-container matColumnDef="status">
      <th mat-header-cell *matHeaderCellDef>Status</th>
      <td mat-cell *matCellDef="let e"><span class="chip" [ngClass]="statusClass(e.status)">{{statusLabel(e.status)}}</span></td>
    </ng-container>
    <ng-container matColumnDef="actions">
      <th mat-header-cell *matHeaderCellDef></th>
      <td mat-cell *matCellDef="let e">
        <button mat-icon-button (click)="openForm(e)" [disabled]="e.status===2"><mat-icon>edit</mat-icon></button>
        <button mat-icon-button color="warn" (click)="remove(e)"><mat-icon>delete</mat-icon></button>
      </td>
    </ng-container>
    <tr mat-header-row *matHeaderRowDef="cols"></tr>
    <tr mat-row *matRowDef="let row; columns: cols;"></tr>
  </table>
  `,
  styles: [`.toolbar{display:flex;gap:1rem;align-items:center;margin-bottom:1rem}.flex-spacer{flex:1}.full-width{width:100%}.chip{padding:2px 8px;border-radius:12px;font-size:12px}.chip.draft{background:#eee}.chip.approved{background:#c8f7c5}.chip.transferred{background:#ffd6a5}`]
})
export class EventsListComponent {
  private service = inject(EventsService);
  private dialog = inject(MatDialog);
  items = signal<EventDto[]>([]);
  search = '';
  statusFilter: string|number = '';
  private searchTimeout: any;
  total = 0;
  debouncedLoad(){ clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(()=> this.load(), 300); }
  load(){ this.service.list(this.search,this.page,this.pageSize).subscribe(r=> {
      this.total = r.total;
      let items = r.items;
      if(this.statusFilter!=='' && this.statusFilter!==undefined){ items = items.filter(i=> i.status=== +this.statusFilter); }
      this.items.set(items);
    }); }
  openForm(e?: EventDto){
    const ref = this.dialog.open(EventFormComponent, { data: e });
    ref.afterClosed().subscribe(ok=>{ if(ok) this.load(); });
  }
  remove(e: EventDto){ if(!confirm('Wirklich löschen?')) return; this.service.delete(e.id!).subscribe(()=> this.load()); }
  statusLabel(s?: number){ return s===0? 'Draft': s===1? 'Approved': s===2? 'Transferred':'?'; }
  statusClass(s?: number){ return s===0? 'chip draft': s===1? 'chip approved': s===2? 'chip transferred':'chip'; }
  page = 1;
  pageSize = 50;
  cols: string[] = ['title','status','actions'];
  prev(){ if(this.page>1){ this.page--; this.load(); } }
  next(){ if(this.page * this.pageSize < this.total){ this.page++; this.load(); } }
  ngOnInit(){ this.load(); }
}
