import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';

import { ContactsService } from '../api/api/contacts.service';
import { EventsService } from '../api/api/events.service';
import { OrganizationsService } from '../api/api/organizations.service';
import { Dashboard } from './dashboard';

describe('Dashboard', () => {
  let component: Dashboard;
  let fixture: ComponentFixture<Dashboard>;
  const eventsServiceStub = {
    apiEventsGet: (_name?: string, status?: number) => of({
      items: status === undefined ? [] : [],
      page: 1,
      pageSize: 1,
      totalCount: 0,
      totalPages: 0,
    }),
  };
  const contactsServiceStub = {
    apiContactsGet: () => of([]),
  };
  const organizationsServiceStub = {
    apiOrganizationsGet: () => of([]),
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Dashboard],
      providers: [
        provideRouter([]),
        { provide: EventsService, useValue: eventsServiceStub },
        { provide: ContactsService, useValue: contactsServiceStub },
        { provide: OrganizationsService, useValue: organizationsServiceStub },
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(Dashboard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
