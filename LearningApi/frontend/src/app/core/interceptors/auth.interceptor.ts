import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

// Prevent multiple redirects on parallel 401s
let isRedirecting = false;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const toast = inject(ToastService);

  // Skip auth endpoints
  if (req.url.includes('/login') || req.url.includes('/register')) {
    return next(req);
  }

  const token = authService.getToken();

  const authReq = token
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {

      // 401 → logout + single redirect
      if (error.status === 401) {
        if (!isRedirecting) {
          isRedirecting = true;
          authService.logout();
          router.navigate(['/login']).finally(() => {
            isRedirecting = false;
          });
        }
        return throwError(() => error);
      }

      // 400 → backend message
      if (error.status === 400) {
        const msg =
          error.error?.message ||
          error.error?.title ||
          'Bad request';
        toast.error(msg);
        return throwError(() => error);
      }

      // 500 → server error
      if (error.status === 500) {
        toast.error('Server error. Try again later.');
        return throwError(() => error);
      }

      // Network error
      if (!error.status) {
        toast.error('Network error. Check connection.');
        return throwError(() => error);
      }

      // Fallback
      toast.error('Something went wrong');
      return throwError(() => error);
    })
  );
};