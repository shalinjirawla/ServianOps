import { TestBed } from '@angular/core/testing';
import { TokenService } from './token.service';
import { StorageService } from './storage.service';
import { vi } from 'vitest';

describe('TokenService', () => {
  let service: TokenService;
  let storageMock: any;

  beforeEach(() => {
    storageMock = {
      getSessionItem: vi.fn(),
      setSessionItem: vi.fn(),
      removeSessionItem: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        TokenService,
        { provide: StorageService, useValue: storageMock }
      ]
    });
    service = TestBed.inject(TokenService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('#decodeToken', () => {
    it('should decode a valid JWT structure and extract role and tenant claims', () => {
      // Mocked JWT payload:
      // {
      //   "sub": "123",
      //   "email": "admin@servianops.com",
      //   "tenant_id": "99",
      //   "user_id": "123",
      //   "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "SuperAdmin",
      //   "exp": 1774349195
      // }
      const mockToken =
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.' +
        'eyJzdWIiOiIxMjMiLCJlbWFpbCI6ImFkbWluQHNlcnZpYW5vcHMuY29tIiwidGVuYW50X2lkIjoiOTkiLCJ1c2VyX2lkIjoiMTIzIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiU3VwZXJBZG1pbiIsImV4cCI6MTc3NDM0OTE5NX0.' +
        'signature_part';

      const decoded = service.decodeToken(mockToken);

      expect(decoded).toBeTruthy();
      expect(decoded?.user_id).toBe('123');
      expect(decoded?.email).toBe('admin@servianops.com');
      expect(decoded?.tenant_id).toBe('99');
      expect(decoded?.role).toBe('SuperAdmin');
      expect(decoded?.exp).toBe(1774349195);
    });

    it('should return null for malformed tokens', () => {
      expect(service.decodeToken('invalid_token')).toBeNull();
    });
  });

  describe('#isTokenExpired', () => {
    it('should identify expired tokens', () => {
      // exp: 1000 (Unix timestamp in the past)
      const mockToken =
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.' +
        'eyJzdWIiOiIxMjMiLCJlbWFpbCI6ImFkbWluQHNlcnZpYW5vcHMuY29tIiwiZXhwIjoxMDAwfQ.' +
        'signature';

      expect(service.isTokenExpired(mockToken)).toBe(true);
    });

    it('should identify valid non-expired tokens', () => {
      // exp: 9999999999 (Unix timestamp far in the future)
      const mockToken =
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.' +
        'eyJzdWIiOiIxMjMiLCJlbWFpbCI6ImFkbWluQHNlcnZpYW5vcHMuY29tIiwiZXhwIjo5OTk5OTk5OTk5fQ.' +
        'signature';

      expect(service.isTokenExpired(mockToken)).toBe(false);
    });
  });
});
