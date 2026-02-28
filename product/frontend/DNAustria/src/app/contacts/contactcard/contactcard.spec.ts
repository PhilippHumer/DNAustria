import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Contactcard } from './contactcard';

describe('Contactcard', () => {
  let component: Contactcard;
  let fixture: ComponentFixture<Contactcard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Contactcard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Contactcard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
