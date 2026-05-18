import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { PagedResult } from '../models/api-response';
import { environment } from '../../../environments/environment';
import { AdminAuditLog } from '../models/admin-audit-logs';
import { BaseApiService } from './base-api.service';

@Injectable({ providedIn: 'root' })
export class AdminAuditLogService extends BaseApiService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/admin-audit-logs`;
  private totalCountSubject = new BehaviorSubject<number>(0);
  public totalCount$ = this.totalCountSubject.asObservable();

  constructor(http: HttpClient) {
    super(http);
  }

  fetchLogs(
    page: number = 1,
    pageSize: number = 20,
    search: string = '',
  ): Observable<AdminAuditLog[]> {
    const params = `page=${page}&pageSize=${pageSize}&search=${encodeURIComponent(search.trim())}`;

    return this.get<PagedResult<AdminAuditLog>>(`${this.apiUrl}?${params}`).pipe(
      tap((result) => {
        this.totalCountSubject.next(result.totalCount);
      }),
      map((result) => result.items || []),
    );
  }
}
