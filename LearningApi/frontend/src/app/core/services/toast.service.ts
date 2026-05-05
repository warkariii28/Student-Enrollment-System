import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'info';

export interface Toast {
  message: string;
  type: ToastType;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly _toast = signal<Toast | null>(null);
  readonly toast = this._toast.asReadonly();

  // frontend/src/app/core/services/toast.service.ts
  show(message: string, type: ToastType = 'info', duration = 3000) {
    this._toast.set({ message, type });

    setTimeout(() => {
      this._toast.set(null);
    }, duration);
  }

  success(msg: string) {
    this.show(msg, 'success');
  }
  error(msg: string) {
    this.show(msg, 'error');
  }
  info(msg: string) {
    this.show(msg, 'info');
  }
}
