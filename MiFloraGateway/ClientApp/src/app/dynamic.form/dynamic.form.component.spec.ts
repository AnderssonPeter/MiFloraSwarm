import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { Dynamic.FormComponent } from './dynamic.form.component';

describe('Dynamic.FormComponent', () => {
  let component: Dynamic.FormComponent;
  let fixture: ComponentFixture<Dynamic.FormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Dynamic.FormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Dynamic.FormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
