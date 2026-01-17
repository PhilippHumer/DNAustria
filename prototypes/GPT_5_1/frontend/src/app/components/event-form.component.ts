import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { EventsService, EventDto } from '../services/events.service';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule, MatCheckboxModule, MatDialogModule],
  template: `
  <h2 mat-dialog-title>{{data? 'Bearbeiten' : 'Neues Event'}}</h2>
  <form [formGroup]="form" (ngSubmit)="save()" class="form">
    <mat-form-field appearance="outline">
      <mat-label>Titel</mat-label>
      <input matInput formControlName="title" required>
      <mat-error *ngIf="form.get('title')?.invalid">Titel ist erforderlich</mat-error>
    </mat-form-field>
    <mat-form-field appearance="outline">
      <mat-label>Beschreibung</mat-label>
      <textarea matInput rows="3" formControlName="description"></textarea>
    </mat-form-field>
    <mat-form-field appearance="outline">
      <mat-label>Status</mat-label>
      <mat-select formControlName="status" [disabled]="isTransferred">
        <mat-option [value]="0">Draft</mat-option>
        <mat-option [value]="1">Approved</mat-option>
        <mat-option [value]="2">Transferred</mat-option>
      </mat-select>
    </mat-form-field>
    <mat-form-field appearance="outline">
      <mat-label>Start</mat-label>
      <input matInput type="datetime-local" formControlName="dateStart">
    </mat-form-field>
    <mat-form-field appearance="outline">
      <mat-label>Ende</mat-label>
      <input matInput type="datetime-local" formControlName="dateEnd">
    </mat-form-field>
    <mat-form-field appearance="outline">
      <mat-label>Topics (Komma getrennt)</mat-label>
      <input matInput formControlName="topics">
    </mat-form-field>
    <mat-form-field appearance="outline">
      <mat-label>Target Audience (Komma getrennt)</mat-label>
      <input matInput formControlName="targetAudience">
    </mat-form-field>
    <mat-checkbox formControlName="isOnline">Online</mat-checkbox>
    <mat-form-field appearance="outline">
      <mat-label>Event Link</mat-label>
      <input matInput formControlName="eventLink">
    </mat-form-field>
    <div class="actions">
      <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">Speichern</button>
      <button mat-button type="button" (click)="cancel()">Abbrechen</button>
    </div>
  </form>
  `,
  styles: [`.form{display:grid;grid-template-columns:repeat(auto-fill,minmax(240px,1fr));gap:1rem}.actions{grid-column:1/-1;display:flex;gap:.5rem}`]
})
export class EventFormComponent {
  form: FormGroup;
  constructor(private fb: FormBuilder, private events: EventsService, private ref: MatDialogRef<EventFormComponent>, @Inject(MAT_DIALOG_DATA) public data: EventDto | null){
    this.form = this.fb.group({
      title: [data?.title || '', Validators.required],
      description: [data?.description || ''],
      dateStart: [data?.dateStart || null],
      dateEnd: [data?.dateEnd || null],
      status: [data?.status ?? 0],
      topics: [data?.topics?.join(',') || ''],
      targetAudience: [data?.targetAudience?.join(',') || ''],
      eventLink: [data?.eventLink || ''],
      isOnline: [data?.isOnline || false]
    });
  }
  save(){
    const v = this.form.value;
    // Neuer TypeScript-Parser statt C# LINQ:
    const parseList = (s?: string | null): number[] | null => {
      if (!s) return null;
      const numbers = s
        .split(',')
        .map(x => x.trim())
        .filter(x => x.length > 0)
        .map(x => {
          const n = Number(x);
          return Number.isFinite(n) ? n : null;
        })
        .filter((n): n is number => n !== null);
      return numbers.length ? numbers : null;
    };
    const dto: EventDto = {
      ...this.data,
      title: v.title!,
      description: v.description || null,
      dateStart: v.dateStart? new Date(v.dateStart).toISOString() : null,
      dateEnd: v.dateEnd? new Date(v.dateEnd).toISOString() : null,
      status: v.status,
      topics: parseList(v.topics),
      targetAudience: parseList(v.targetAudience),
      eventLink: v.eventLink || null,
      isOnline: !!v.isOnline
    } as EventDto;
    const obs = dto.id ? this.events.update(dto.id, dto) : this.events.create(dto);
    obs.subscribe(()=> this.ref.close(true));
  }
  cancel(){ this.ref.close(false); }
  get isTransferred(){ return this.data?.status===2 || this.form.get('status')?.value===2; }
}
