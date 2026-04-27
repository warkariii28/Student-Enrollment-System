import { Injectable, signal } from '@angular/core';

interface ConfirmState {
  message: string;
  resolve: (value: boolean) => void;
}

@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private readonly _state = signal<ConfirmState | null>(null);
  readonly state = this._state.asReadonly();

  ask(message: string): Promise<boolean> {
    return new Promise<boolean>((resolve) => {
      this._state.set({ message, resolve });
    });
  }

  confirm(): void {
    const current = this._state();
    if (!current) return;

    current.resolve(true);
    this._state.set(null);
  }

  cancel(): void {
    const current = this._state();
    if (!current) return;

    current.resolve(false);
    this._state.set(null);
  }
}