import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';

interface ExportEvent {
  event_title: string;
  event_start?: string|null;
  event_end?: string|null;
  organization_name?: string|null;
  event_is_online: boolean;
}

@Component({
  standalone: true,
  selector: 'app-public-export',
  imports: [CommonModule, MatTableModule],
  template: `<h2>Public Export</h2>
  <table mat-table [dataSource]="items()" class="mat-elevation-z1">
    <ng-container matColumnDef="title"><th mat-header-cell *matHeaderCellDef>Title</th><td mat-cell *matCellDef="let e">{{e.event_title}}</td></ng-container>
    <ng-container matColumnDef="start"><th mat-header-cell *matHeaderCellDef>Start</th><td mat-cell *matCellDef="let e">{{e.event_start | date:'short'}}</td></ng-container>
    <ng-container matColumnDef="org"><th mat-header-cell *matHeaderCellDef>Organization</th><td mat-cell *matCellDef="let e">{{e.organization_name}}</td></ng-container>
    <ng-container matColumnDef="online"><th mat-header-cell *matHeaderCellDef>Online</th><td mat-cell *matCellDef="let e">{{e.event_is_online ? 'Yes':'No'}}</td></ng-container>
    <tr mat-header-row *matHeaderRowDef="cols"></tr>
    <tr mat-row *matRowDef="let row; columns: cols;"></tr>
  </table>`
})
export class PublicExportComponent {
  private http = new HttpClient({} as any); // will be intercepted
  items = signal<ExportEvent[]>([]);
  cols = ['title','start','org','online'];
  ngOnInit(){ this.http.get<ExportEvent[]>('/server/api/public/events').subscribe(r=> this.items.set(r)); }
}

