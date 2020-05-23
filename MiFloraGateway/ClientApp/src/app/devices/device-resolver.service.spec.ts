import { TestBed } from '@angular/core/testing';

import { DeviceResolverService } from './device-resolver.service';

describe('DeviceResolverService', () => {
  let service: DeviceResolverService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeviceResolverService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
