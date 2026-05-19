import { TestBed } from '@angular/core/testing';
import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';

import { AuthService } from './auth.service';

describe('AuthService', () => {
  const httpMock = {
    post: vi.fn()
  };

  beforeEach(() => {
    localStorage.clear();
    vi.clearAllMocks();

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: HttpClient, useValue: httpMock }
      ]
    });
  });

  it('stores token, refresh token, and user when login succeeds', () => {
    httpMock.post.mockReturnValue(of({
      success: true,
      message: 'Login successful',
      data: {
        token: 'access-token',
        refreshToken: 'refresh-token',
        user: {
          userId: 1,
          name: 'Admin',
          email: 'admin@example.com',
          role: 'Admin'
        }
      }
    }));

    const service = TestBed.inject(AuthService);

    service.login({
      email: 'admin@example.com',
      password: 'Password123'
    }).subscribe((token) => {
      expect(token).toBe('access-token');
      expect(service.getToken()).toBe('access-token');
      expect(service.getRefreshToken()).toBe('refresh-token');
      expect(service.getUser()).toEqual({
        userId: 1,
        name: 'Admin',
        email: 'admin@example.com',
        role: 'Admin'
      });
    });

    expect(httpMock.post).toHaveBeenCalled();
  });

  it('clears token, refresh token, and user on logout', () => {
    const service = TestBed.inject(AuthService);

    service.setToken('access-token');
    service.setRefreshToken('refresh-token');
    service.setUser({
      userId: 1,
      name: 'Admin',
      email: 'admin@example.com',
      role: 'Admin'
    });

    service.logout();

    expect(service.getToken()).toBeNull();
    expect(service.getRefreshToken()).toBeNull();
    expect(service.getUser()).toBeNull();
  });
});