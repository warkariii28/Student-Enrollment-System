import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { API_BASE_URL } from '../api.config';
import { Student, StudentPayload } from '../models/student';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class StudentService {
  private readonly apiUrl = `${API_BASE_URL}/api/students`;

  constructor(private readonly http: HttpClient) {}

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

  deleteStudent(id: number): Observable<string> {
    return this.http
      .delete<ApiResponse<null>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.message));
  }
}