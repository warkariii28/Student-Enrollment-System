import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../models/api-response';

export class BaseApiService {

  constructor(protected http: HttpClient) {}

  protected get<T>(url: string): Observable<T> {
    return this.http
      .get<ApiResponse<T>>(url)
      .pipe(map(res => res.data));
  }

  protected post<T>(url: string, body: any): Observable<T> {
    return this.http
      .post<ApiResponse<T>>(url, body)
      .pipe(map(res => res.data));
  }

  protected putWithMessage(url: string, body: any): Observable<string> {
    return this.http
      .put<ApiResponse<null>>(url, body)
      .pipe(map(res => res.message));
  }

  protected delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(url);
  }
}