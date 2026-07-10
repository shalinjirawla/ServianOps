import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="fixed top-4 right-4 z-[100] flex flex-col gap-2 w-full max-w-sm">
      @for (t of toastSvc.toasts(); track t.id) {
        <div
          class="rounded-lg border shadow-elegant px-4 py-3 text-sm bg-card text-card-foreground flex items-center gap-2"
          [class.border-success]="t.kind === 'success'"
          [class.border-destructive]="t.kind === 'error'"
        >
          <span
            class="w-2 h-2 rounded-full shrink-0"
            [class.bg-success]="t.kind === 'success'"
            [class.bg-destructive]="t.kind === 'error'"
          ></span>
          {{ t.text }}
        </div>
      }
    </div>
  `,
})
export class ToastComponent {
  constructor(public toastSvc: ToastService) {}
}
