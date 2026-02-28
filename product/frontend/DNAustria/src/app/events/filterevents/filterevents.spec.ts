import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Filterevents } from './filterevents';

describe('Filterevents', () => {
  let component: Filterevents;
  let fixture: ComponentFixture<Filterevents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Filterevents]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Filterevents);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
