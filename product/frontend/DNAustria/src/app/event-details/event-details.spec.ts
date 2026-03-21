import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';

import { EventDetails } from './event-details';
import { ContactsService } from '../api/api/contacts.service';
import { EventsService } from '../api/api/events.service';
import { LocationsService } from '../api/api/locations.service';
import { OrganizationsService } from '../api/api/organizations.service';

describe('EventDetails', () => {
  let component: EventDetails;
  let fixture: ComponentFixture<EventDetails>;

  const event = {
    id: 1,
    name: 'Test Event',
    description: '',
    link: '',
    startDate: new Date().toISOString(),
    endDate: new Date().toISOString(),
    classification: 0,
    status: 0,
    hasFees: false,
    isOnline: true,
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
      imports: [EventDetails],
      providers: [
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ id: '1' })) } },
        { provide: EventsService, useValue: { apiEventsIdGet: () => of(event), apiEventsIdDelete: () => of(void 0) } },
        { provide: OrganizationsService, useValue: { apiOrganizationsIdGet: () => of(null) } },
        { provide: ContactsService, useValue: { apiContactsIdGet: () => of(null) } },
        { provide: LocationsService, useValue: { locationsIdGet: () => of(null) } },
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
