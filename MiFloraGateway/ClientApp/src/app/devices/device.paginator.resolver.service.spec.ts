import { TestBed } from '@angular/core/testing';

import { DevicePaginatorResolverService } from './device.paginator.resolver.service';

describe('DevicePaginatorResolverService', () => {
  let service: DevicePaginatorResolverService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DevicePaginatorResolverService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
