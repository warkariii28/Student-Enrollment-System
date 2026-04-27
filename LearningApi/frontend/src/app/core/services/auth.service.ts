import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, tap } from 'rxjs';

import { API_BASE_URL } from '../api.config';
import { LoginRequest, LoginResponse, RegisterRequest } from '../models/user';
import { ApiResponse } from '../models/api-response';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'jwt_token';

  constructor(private readonly http: HttpClient) { }

  login(credentials: LoginRequest): Observable<string> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${API_BASE_URL}/api/login`, credentials)
      .pipe(
        tap(res => this.setToken(res.data.token)),
        map(res => res.data.token)
      );
  }

  register(user: RegisterRequest): Observable<string> {
    return this.http.post(`${API_BASE_URL}/api/register`, user, { responseType: 'text' });
  }

  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000;

      return Date.now() < expiry;
    } catch {
      return false;
    }
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
  }
}
