import { TestBed, inject } from '@angular/core/testing';

import { ApiInfoService } from './api-info.service';

describe('ApiInfoService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ApiInfoService]
    });
  });

  it('should be created', inject([ApiInfoService], (service: ApiInfoService) => {
    expect(service).toBeTruthy();
  }));
});
