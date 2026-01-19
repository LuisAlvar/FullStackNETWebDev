import { TestBed } from '@angular/core/testing';

import { ImagerAppService } from './imager-app.service';

describe('ImagerAppService', () => {
  let service: ImagerAppService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ImagerAppService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
