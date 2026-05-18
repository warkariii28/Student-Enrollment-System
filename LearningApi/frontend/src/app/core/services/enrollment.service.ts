import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap, of, shareReplay, finalize, map } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Enrollment, EnrollmentPayload } from '../models/enrollment';
import { PagedResult } from '../models/api-response';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class EnrollmentService extends BaseApiService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/enrollments`;

  private enrollmentsSubject = new BehaviorSubject<Enrollment[]>([]);
  public enrollments$ = this.enrollmentsSubject.asObservable();

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  private inflight$?: Observable<Enrollment[]>;

  constructor(http: HttpClient) {
    super(http);
  }

  fetchEnrollments(forceRefresh: boolean = false): Observable<Enrollment[]> {
    const current = this.enrollmentsSubject.value;

    if (!forceRefresh && current.length > 0) {
      return of(current);
    }

    if (this.inflight$ && !forceRefresh) {
      return this.inflight$;
    }

    this.loadingSubject.next(true);

    this.inflight$ = this.get<PagedResult<Enrollment>>(this.apiUrl).pipe(
      map((result) => result.items || []),
      tap((enrollments) => {
        this.enrollmentsSubject.next(enrollments);
      }),
      finalize(() => {
        this.loadingSubject.next(false);
        this.inflight$ = undefined;
      }),
      shareReplay(1),
    );

    return this.inflight$;
  }

  hasEnrollments(): boolean {
    return this.enrollmentsSubject.value.length > 0;
  }

  addEnrollment(payload: EnrollmentPayload): Observable<number> {
    return this.post<number>(this.apiUrl, payload).pipe(
      tap(() => {
        // must refetch (backend returns incomplete object)
        this.fetchEnrollments(true).subscribe();
      }),
    );
  }

  updateEnrollment(id: number, payload: EnrollmentPayload): Observable<string> {
    return this.putWithMessage(`${this.apiUrl}/${id}`, payload).pipe(
      tap(() => {
        this.fetchEnrollments(true).subscribe();
      }),
    );
  }

  deleteEnrollment(id: number): Observable<void> {
    return this.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        this.fetchEnrollments(true).subscribe();
      }),
    );
  }
}
