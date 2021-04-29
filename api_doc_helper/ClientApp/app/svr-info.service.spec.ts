import { TestBed, inject } from '@angular/core/testing';

import { SvrInfoService } from './svr-info.service';

describe('SvrInfoService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SvrInfoService]
    });
  });

  it('should be created', inject([SvrInfoService], (service: SvrInfoService) => {
    expect(service).toBeTruthy();
  }));
});
