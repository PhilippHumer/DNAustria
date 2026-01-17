import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganizationForm } from './organization-form';

describe('OrganizationForm', () => {
  let component: OrganizationForm;
  let fixture: ComponentFixture<OrganizationForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrganizationForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OrganizationForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
