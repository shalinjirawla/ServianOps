import { Injectable, signal } from '@angular/core';

export interface ToastMsg {
  id: number;
  text: string;
  kind: 'success' | 'error' | 'warning';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  toasts = signal<ToastMsg[]>([]);
  private nextId = 1;

  private push(text: string, kind: 'success' | 'error' | 'warning') {
    const id = this.nextId++;
    this.toasts.update((t) => [...t, { id, text, kind }]);
    setTimeout(() => this.dismiss(id), 3000);
  }

  success(text: string) {
    this.push(text, 'success');
  }

  error(text: string) {
    this.push(text, 'error');
  }

  warning(text: string) {
    this.push(text, 'warning');
  }

  dismiss(id: number) {
    this.toasts.update((t) => t.filter((x) => x.id !== id));
  }
}
