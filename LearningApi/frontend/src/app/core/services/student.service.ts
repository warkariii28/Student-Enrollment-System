import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, BehaviorSubject, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Student, StudentPayload } from '../models/student';
import { ApiResponse } from '../models/api-response';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/students`;

  // State
  private studentsSubject = new BehaviorSubject<Student[]>([]);
  public students$ = this.studentsSubject.asObservable();

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  constructor(private readonly http: HttpClient) { }

  fetchStudents(forceRefresh: boolean = false): Observable<Student[]> {
    const current = this.studentsSubject.value;

    // Skip API if already loaded
    if (!forceRefresh && current.length > 0) {
      return new Observable(observer => {
        observer.next(current);
        observer.complete();
      });
    }

    this.loadingSubject.next(true);

    return this.http
      .get<ApiResponse<Student[]>>(this.apiUrl)
      .pipe(
        map(res => res.data),
        tap((students) => {
          this.studentsSubject.next(students || []);
          this.loadingSubject.next(false);
        })
      );
  }

  hasStudents(): boolean {
    return this.studentsSubject.value.length > 0;
  }

  getStudent(id: number): Observable<Student> {
    return this.http
      .get<ApiResponse<Student>>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.data));
  }

  addStudent(student: StudentPayload): Observable<number> {
    return this.http
      .post<ApiResponse<number>>(this.apiUrl, student)
      .pipe(
        map(res => res.data),
        tap((id) => {
          const newStudent: Student = {
            studentID: id,
            ...student
          };

          const current = this.studentsSubject.value;
          this.studentsSubject.next([newStudent, ...current]);
        })
      );
  }

  updateStudent(id: number, student: StudentPayload): Observable<string> {
    return this.http
      .put<ApiResponse<null>>(`${this.apiUrl}/${id}`, student)
      .pipe(
        map(res => res.message),
        tap(() => {
          const updatedList = this.studentsSubject.value.map(s =>
            s.studentID === id ? { ...s, ...student } : s
          );
          this.studentsSubject.next(updatedList);
        })
      );
  }

  deleteStudent(id: number): Observable<void> {
    return this.http
      .delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        tap(() => {
          const filtered = this.studentsSubject.value.filter(
            s => s.studentID !== id
          );
          this.studentsSubject.next(filtered);
        })
      );
  }
}