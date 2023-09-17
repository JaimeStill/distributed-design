import { TestBed } from '@angular/core/testing';

import { ToolkitService } from './toolkit.service';

describe('ToolkitService', () => {
  let service: ToolkitService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ToolkitService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
