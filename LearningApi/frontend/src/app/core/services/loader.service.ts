import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoaderService {
  private readonly _count = signal(0);

  readonly loading = signal(false);

  show(): void {
    this._count.update(c => c + 1);
    this.loading.set(true);
  }

  hide(): void {
    this._count.update(c => Math.max(0, c - 1));
    if (this._count() === 0) {
      this.loading.set(false);
    }
  }
}