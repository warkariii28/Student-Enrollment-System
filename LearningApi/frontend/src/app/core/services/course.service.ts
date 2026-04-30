import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, of, shareReplay } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Course, CoursePayload } from '../models/course';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class CourseService extends BaseApiService {

  private readonly apiUrl = `${environment.apiBaseUrl}/api/courses`;

  private coursesSubject = new BehaviorSubject<Course[]>([]);
  public courses$ = this.coursesSubject.asObservable();

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  private inflight$?: Observable<Course[]>;

  constructor(http: HttpClient) {
    super(http);
  }

  fetchCourses(forceRefresh: boolean = false): Observable<Course[]> {
    const current = this.coursesSubject.value;

    if (!forceRefresh && current.length > 0) {
      return of(current);
    }

    if (this.inflight$ && !forceRefresh) {
      return this.inflight$;
    }

    this.loadingSubject.next(true);

    this.inflight$ = this.get<Course[]>(this.apiUrl).pipe(
      shareReplay(1),
      tap((courses) => {
        this.coursesSubject.next(courses || []);
        this.loadingSubject.next(false);
        this.inflight$ = undefined;
      })
    );

    return this.inflight$;
  }

  hasCourses(): boolean {
    return this.coursesSubject.value.length > 0;
  }

  getCourse(id: number): Observable<Course> {
    return this.get<Course>(`${this.apiUrl}/${id}`);
  }

  addCourse(payload: CoursePayload): Observable<number> {
    return this.post<number>(this.apiUrl, payload).pipe(
      tap((courseID) => {
        const newCourse: Course = { courseID, ...payload };
        this.coursesSubject.next([newCourse, ...this.coursesSubject.value]);
      })
    );
  }

  updateCourse(id: number, payload: CoursePayload): Observable<string> {
    return this.putWithMessage(`${this.apiUrl}/${id}`, payload).pipe(
      tap(() => {
        const updated = this.coursesSubject.value.map(c =>
          c.courseID === id ? { ...c, ...payload } : c
        );
        this.coursesSubject.next(updated);
      })
    );
  }

  deleteCourse(id: number): Observable<void> {
    return this.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        const filtered = this.coursesSubject.value.filter(c => c.courseID !== id);
        this.coursesSubject.next(filtered);
      })
    );
  }
}