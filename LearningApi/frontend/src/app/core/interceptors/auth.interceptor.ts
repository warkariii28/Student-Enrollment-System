import {
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpBackend,
  HttpClient,
  HttpRequest
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, filter, switchMap, take, throwError } from 'rxjs';

import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

// 🔒 shared state (singleton across interceptor calls)
let isRefreshing = false;
const refreshSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const toast = inject(ToastService);

  // 🔴 bypass interceptor for refresh/login/register
  const http = new HttpClient(inject(HttpBackend));

  const isAuthEndpoint =
    req.url.includes('/api/auth/login') ||
    req.url.includes('/api/auth/register') ||
    req.url.includes('/api/auth/refresh');

  if (isAuthEndpoint) {
    return next(req);
  }

  // attach access token
  const token = auth.getToken();
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      // ❌ do not retry if already retried
      if (authReq.headers.has('x-retry')) {
        auth.logout();
        router.navigate(['/login']);
        return throwError(() => error);
      }

      const refreshToken = auth.getRefreshToken();
      if (!refreshToken) {
        auth.logout();
        router.navigate(['/login']);
        return throwError(() => error);
      }

      // 🔁 if refresh already in progress → queue requests
      if (isRefreshing) {
        return refreshSubject.pipe(
          filter(token => token !== null),
          take(1),
          switchMap(newToken => {
            const retryReq = addAuthHeader(authReq, newToken!, true);
            return next(retryReq);
          })
        );
      }

      // 🔓 start refresh
      isRefreshing = true;
      refreshSubject.next(null);

      return http
        .post<any>(`${auth['apiUrl']}/refresh`, { refreshToken })
        .pipe(
          switchMap(res => {
            const newToken = res.data.token;
            const newRefresh = res.data.refreshToken;

            auth.setToken(newToken);
            auth.setRefreshToken(newRefresh);

            isRefreshing = false;
            refreshSubject.next(newToken);

            const retryReq = addAuthHeader(authReq, newToken, true);
            return next(retryReq);
          }),
          catchError(err => {
            isRefreshing = false;
            auth.logout();
            router.navigate(['/login']);
            return throwError(() => err);
          })
        );
    })
  );
};

// 🔧 helper
function addAuthHeader(req: HttpRequest<any>, token: string, retried = false) {
  return req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`,
      ...(retried ? { 'x-retry': 'true' } : {})
    }
  });
}