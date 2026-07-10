import { Injectable, signal } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export interface ConfirmState {
  title: string;
  message: string;
  confirmBtnText?: string;
  cancelBtnText?: string;
}

@Injectable({
  providedIn: 'root',
})
export class ConfirmationModalService {
  state = signal<ConfirmState | null>(null);
  private resultSubject = new Subject<boolean>();

  confirm(title: string, message: string, confirmBtnText = 'Delete', cancelBtnText = 'Cancel'): Observable<boolean> {
    this.state.set({ title, message, confirmBtnText, cancelBtnText });
    this.resultSubject = new Subject<boolean>();
    return this.resultSubject.asObservable();
  }

  resolve(value: boolean): void {
    this.resultSubject.next(value);
    this.resultSubject.complete();
    this.state.set(null);
  }
}
