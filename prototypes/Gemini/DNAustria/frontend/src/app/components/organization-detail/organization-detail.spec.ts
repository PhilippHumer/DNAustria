import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationDetail } from './organization-detail';

describe('OrganizationDetail', () => {
  let component: OrganizationDetail;
  let fixture: ComponentFixture<OrganizationDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrganizationDetail]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrganizationDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
