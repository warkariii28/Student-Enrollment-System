import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Student, StudentPayload } from '../models/student';
import { ApiResponse } from '../models/api-response';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/students`;

  constructor(private readonly http: HttpClient) { }

  getStudents(): Observable<Student[]> {
    return this.http
      .get<ApiResponse<Student[]>>(this.apiUrl)
      .pipe(map(res => res.data));
  }

  getStudent(id: number): Observable<Student> {
    return this.http
      .get<ApiResponse<Student>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.data));
  }

  addStudent(student: StudentPayload): Observable<number> {
    return this.http
      .post<ApiResponse<number>>(this.apiUrl, student)
      .pipe(map(res => res.data));
  }

  updateStudent(id: number, student: StudentPayload): Observable<string> {
    return this.http
      .put<ApiResponse<null>>(`${this.apiUrl}/${id}`, student)
      .pipe(map(res => res.message));
  }

  deleteStudent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}