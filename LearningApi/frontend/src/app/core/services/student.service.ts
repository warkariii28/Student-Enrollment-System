import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, of, shareReplay } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Student, StudentPayload } from '../models/student';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class StudentService extends BaseApiService {

  private readonly apiUrl = `${environment.apiBaseUrl}/api/students`;

  private studentsSubject = new BehaviorSubject<Student[]>([]);
  public students$ = this.studentsSubject.asObservable();

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  private inflight$?: Observable<Student[]>;

  constructor(http: HttpClient) {
    super(http);
  }

  fetchStudents(forceRefresh: boolean = false): Observable<Student[]> {
    const current = this.studentsSubject.value;

    if (!forceRefresh && current.length > 0) {
      return of(current);
    }

    if (this.inflight$ && !forceRefresh) {
      return this.inflight$;
    }

    this.loadingSubject.next(true);

    this.inflight$ = this.get<Student[]>(this.apiUrl).pipe(
      shareReplay(1),
      tap((students) => {
        this.studentsSubject.next(students || []);
        this.loadingSubject.next(false);
        this.inflight$ = undefined;
      })
    );

    return this.inflight$;
  }

  hasStudents(): boolean {
    return this.studentsSubject.value.length > 0;
  }

  getStudent(id: number): Observable<Student> {
    return this.get<Student>(`${this.apiUrl}/${id}`);
  }

  addStudent(student: StudentPayload): Observable<number> {
    return this.post<number>(this.apiUrl, student).pipe(
      tap((id) => {
        const newStudent: Student = { studentID: id, ...student };
        this.studentsSubject.next([newStudent, ...this.studentsSubject.value]);
      })
    );
  }

  updateStudent(id: number, student: StudentPayload): Observable<string> {
    return this.putWithMessage(`${this.apiUrl}/${id}`, student).pipe(
      tap(() => {
        const updated = this.studentsSubject.value.map(s =>
          s.studentID === id ? { ...s, ...student } : s
        );
        this.studentsSubject.next(updated);
      })
    );
  }

  deleteStudent(id: number): Observable<void> {
    return this.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        const filtered = this.studentsSubject.value.filter(
          s => s.studentID !== id
        );
        this.studentsSubject.next(filtered);
      })
    );
  }
}