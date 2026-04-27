import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { API_BASE_URL } from '../api.config';
import { Enrollment, EnrollmentPayload } from '../models/enrollment';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class EnrollmentService {
  private readonly apiUrl = `${API_BASE_URL}/api/enrollments`;

  constructor(private readonly http: HttpClient) {}

  getEnrollments(): Observable<Enrollment[]> {
    return this.http
      .get<ApiResponse<Enrollment[]>>(this.apiUrl)
      .pipe(map(res => res.data));
  }

  addEnrollment(payload: EnrollmentPayload): Observable<string> {
    return this.http
      .post<ApiResponse<null>>(this.apiUrl, payload)
      .pipe(map(res => res.message));
  }

  deleteEnrollment(id: number): Observable<string> {
    return this.http
      .delete<ApiResponse<null>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.message));
  }
}