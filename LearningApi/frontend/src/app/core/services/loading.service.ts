// frontend/src/app/core/services/loading.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject,asyncScheduler  } from 'rxjs';
import { distinctUntilChanged, observeOn } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private activeRequests = 0;
  private loadingSubject = new BehaviorSubject<boolean>(false);

  readonly loading$ = this.loadingSubject
    .asObservable()
    .pipe(distinctUntilChanged(), observeOn(asyncScheduler));

  start() {
    this.activeRequests++;

    if (this.activeRequests === 1) {
      setTimeout(() => this.loadingSubject.next(true), 0);
    }
  }

  stop() {
    if (this.activeRequests > 0) {
      this.activeRequests--;
    }

    if (this.activeRequests === 0) {
      setTimeout(() => this.loadingSubject.next(false), 0);
    }
  }
}
