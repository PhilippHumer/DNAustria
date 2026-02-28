import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Organizationcard } from './organizationcard';

describe('Organizationcard', () => {
  let component: Organizationcard;
  let fixture: ComponentFixture<Organizationcard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Organizationcard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Organizationcard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
