import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { TokenService } from './token.service';
import { environment } from '../../../environments/environment';
import { AuthResponseDto } from '../models/auth.models';
import { vi } from 'vitest';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let tokenServiceMock: any;
  let routerMock: any;

  beforeEach(() => {
    tokenServiceMock = {
      getToken: vi.fn(),
      setToken: vi.fn(),
      clearToken: vi.fn(),
      decodeToken: vi.fn(),
      isTokenExpired: vi.fn()
    };
    routerMock = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: TokenService, useValue: tokenServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created and initiate with null session', () => {
    expect(service).toBeTruthy();
    expect(service.currentUser()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
  });

  describe('#login', () => {
    it('should POST request to login API, save token and decode user context session', () => {
      const mockLoginResponse: AuthResponseDto = {
        token: 'fake_jwt_token',
        userId: 12,
        tenantId: 44,
        email: 'test@servianops.com',
        role: 'Administrator'
      };

      const mockDecoded = {
        sub: '12',
        email: 'test@servianops.com',
        jti: 'jti',
        tenant_id: '44',
        user_id: '12',
        role: 'Administrator',
        exp: 2000000000
      };

      tokenServiceMock.decodeToken.mockReturnValue(mockDecoded);

      service.login({ email: 'test@servianops.com', password: '123' }).subscribe((res) => {
        expect(res).toEqual(mockLoginResponse);
        expect(tokenServiceMock.setToken).toHaveBeenCalledWith('fake_jwt_token');
        expect(service.currentUser()?.email).toBe('test@servianops.com');
        expect(service.userRole()).toBe('Administrator');
        expect(service.tenantId()).toBe(44);
        expect(service.isAuthenticated()).toBe(true);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/api/auth/login`);
      expect(req.request.method).toBe('POST');
      req.flush(mockLoginResponse);
    });
  });

  describe('#logout', () => {
    it('should clear token and reset session signals', () => {
      // Simulate active user state
      service.currentUser.set({
        token: 'fake',
        userId: 1,
        tenantId: null,
        email: 'admin@host.com',
        role: 'SuperAdmin',
        decodedToken: {} as any
      });

      service.logout();

      expect(tokenServiceMock.clearToken).toHaveBeenCalled();
      expect(service.currentUser()).toBeNull();
      expect(service.isAuthenticated()).toBe(false);
      expect(routerMock.navigate).toHaveBeenCalledWith(['/login'], { queryParams: {} });

      const logoutReq = httpMock.expectOne(`${environment.apiUrl}/api/auth/logout`);
      expect(logoutReq.request.method).toBe('POST');
      logoutReq.flush({});
    });
  });
});
