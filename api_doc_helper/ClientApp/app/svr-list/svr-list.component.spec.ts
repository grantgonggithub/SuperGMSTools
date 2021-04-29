import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SvrListComponent } from './svr-list.component';

describe('SvrListComponent', () => {
  let component: SvrListComponent;
  let fixture: ComponentFixture<SvrListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SvrListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SvrListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
