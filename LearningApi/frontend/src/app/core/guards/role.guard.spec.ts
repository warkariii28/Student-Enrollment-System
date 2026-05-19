import { ActivatedRouteSnapshot, Router } from '@angular/router';
import { TestBed } from '@angular/core/testing';

import { roleGuard } from './role.guard';
import { AuthService } from '../services/auth.service';

describe('roleGuard', () => {
  const routerMock = {
    navigate: vi.fn()
  };

  const authServiceMock = {
    getRole: vi.fn()
  };

  beforeEach(() => {
    vi.clearAllMocks();

    TestBed.configureTestingModule({
      providers: [
        { provide: AuthService, useValue: authServiceMock },
        { provide: Router, useValue: routerMock }
      ]
    });
  });

  function routeWithRoles(roles?: string[]): ActivatedRouteSnapshot {
    return {
      data: roles ? { roles } : {}
    } as ActivatedRouteSnapshot;
  }

  it('allows access when no roles are configured', () => {
    const result = TestBed.runInInjectionContext(() =>
      roleGuard(routeWithRoles(), {} as any)
    );

    expect(result).toBe(true);
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('allows access when user role is allowed', () => {
    authServiceMock.getRole.mockReturnValue('Admin');

    const result = TestBed.runInInjectionContext(() =>
      roleGuard(routeWithRoles(['Admin']), {} as any)
    );

    expect(result).toBe(true);
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('redirects to dashboard when user role is not allowed', () => {
    authServiceMock.getRole.mockReturnValue('User');

    const result = TestBed.runInInjectionContext(() =>
      roleGuard(routeWithRoles(['Admin']), {} as any)
    );

    expect(result).toBe(false);
    expect(routerMock.navigate).toHaveBeenCalledWith(['/dashboard']);
  });
});