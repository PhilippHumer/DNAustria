import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Eventcard } from './eventcard';
import { EventDto } from '../../api/model/eventDto';

describe('Eventcard', () => {
  let component: Eventcard;
  let fixture: ComponentFixture<Eventcard>;
  const event: EventDto = {
    id: 1,
    name: 'Test Event',
    description: 'Description',
    link: '',
    startDate: new Date().toISOString(),
    endDate: new Date().toISOString(),
    classification: 0,
    status: 0,
    hasFees: false,
    isOnline: false,
    organization: null,
    programName: '',
    format: '',
    schoolBookable: false,
    ageMinimum: 0,
    ageMaximum: 99,
    location: null,
    contact: null,
    targetAudiences: [],
    topics: [],
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Eventcard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Eventcard);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('event', event);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
