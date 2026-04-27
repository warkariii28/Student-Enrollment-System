import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';

import { API_BASE_URL } from '../api.config';
import { Course, CoursePayload } from '../models/course';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class CourseService {
  private readonly apiUrl = `${API_BASE_URL}/api/courses`;

  constructor(private readonly http: HttpClient) {}

  getCourses(): Observable<Course[]> {
    return this.http
      .get<ApiResponse<Course[]>>(this.apiUrl)
      .pipe(map(res => res.data));
  }

  getCourse(id: number): Observable<Course> {
    return this.http
      .get<ApiResponse<Course>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.data));
  }

  addCourse(course: CoursePayload): Observable<number> {
    return this.http
      .post<ApiResponse<number>>(this.apiUrl, course)
      .pipe(map(res => res.data));
  }

  updateCourse(id: number, course: CoursePayload): Observable<string> {
    return this.http
      .put<ApiResponse<null>>(`${this.apiUrl}/${id}`, course)
      .pipe(map(res => res.message));
  }

  deleteCourse(id: number): Observable<string> {
    return this.http
      .delete<ApiResponse<null>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.message));
  }
}