import { TestBed, inject } from '@angular/core/testing';

import { ApiHelpService } from './api-help.service';

describe('ApiHelpService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ApiHelpService]
    });
  });

  it('should be created', inject([ApiHelpService], (service: ApiHelpService) => {
    expect(service).toBeTruthy();
  }));
});
