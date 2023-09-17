import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ToolkitComponent } from './toolkit.component';

describe('ToolkitComponent', () => {
  let component: ToolkitComponent;
  let fixture: ComponentFixture<ToolkitComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ToolkitComponent]
    });
    fixture = TestBed.createComponent(ToolkitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
