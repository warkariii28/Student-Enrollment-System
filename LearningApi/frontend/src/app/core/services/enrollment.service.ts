import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Enrollment, EnrollmentPayload } from '../models/enrollment';
import { ApiResponse } from '../models/api-response';

@Injectable({ providedIn: 'root' })
export class EnrollmentService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/enrollments`;

  constructor(private readonly http: HttpClient) { }

  getEnrollments(): Observable<Enrollment[]> {
    return this.http
      .get<ApiResponse<Enrollment[]>>(this.apiUrl)
      .pipe(map(res => res.data));
  }

  addEnrollment(payload: EnrollmentPayload): Observable<void> {
    return this.http.post<void>(this.apiUrl, payload);
  }

  deleteEnrollment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}