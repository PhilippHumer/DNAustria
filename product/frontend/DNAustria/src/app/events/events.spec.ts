import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { Events } from './events';
import { EventsService } from '../api/api/events.service';

describe('Events', () => {
  let component: Events;
  let fixture: ComponentFixture<Events>;
  const eventsServiceStub = {
    apiEventsGet: () => of([]),
    apiEventsIdDelete: () => of(void 0),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Events],
      providers: [{ provide: EventsService, useValue: eventsServiceStub }]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Events);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
