import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmationModalService } from './confirmation-modal.service';
import { IconComponent } from '../icon/icon.component';

@Component({
  selector: 'app-confirmation-modal',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './confirmation-modal.component.html',
  styleUrl: './confirmation-modal.component.scss',
})
export class ConfirmationModalComponent {
  constructor(public confirmSvc: ConfirmationModalService) {}

  onConfirm(): void {
    this.confirmSvc.resolve(true);
  }

  onCancel(): void {
    this.confirmSvc.resolve(false);
  }
}
