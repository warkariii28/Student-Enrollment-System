import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';   // ✅ ADD

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const toast = inject(ToastService);   // ✅ ADD

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

      // 🔴 401 → logout
      if (error.status === 401) {
        authService.logout();
        router.navigate(['/login']);
        return throwError(() => error);
      }

      // 🔴 400 → backend message
      if (error.status === 400) {
        const msg = error.error?.message || 'Bad request';
        toast.error(msg);
        return throwError(() => error);
      }

      // 🔴 500 → server error
      if (error.status === 500) {
        toast.error('Server error. Try again later.');
        return throwError(() => error);
      }

      // 🔴 Network / unknown
      if (!error.status) {
        toast.error('Network error. Check connection.');
        return throwError(() => error);
      }

      // fallback
      toast.error('Something went wrong');
      return throwError(() => error);
    })
  );
};