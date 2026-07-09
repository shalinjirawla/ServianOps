import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutService } from '../../core/services/layout.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';

interface SessionItem {
  id: string;
  device: string;
  location: string;
  ip: string;
  current: boolean;
}

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private readonly layoutService = inject(LayoutService);
  readonly authService = inject(AuthService);
  private readonly toastService = inject(ToastService);

  // Profile details model variables
  fullName = 'Administrator User';
  phoneNumber = '+44 7700 900123';
  title = 'Lead Scheduler Operations';

  // Session details registry
  readonly sessions = signal<SessionItem[]>([
    { id: 's1', device: 'Chrome, Windows 11', location: 'London, UK', ip: '82.41.93.18', current: true },
    { id: 's2', device: 'Safari, iPhone 15 Pro', location: 'London, UK', ip: '82.41.93.18', current: false },
    { id: 's3', device: 'Edge, Windows 11', location: 'Manchester, UK', ip: '194.22.181.90', current: false }
  ]);

  ngOnInit(): void {
    this.layoutService.setPageTitle('My Profile');
  }

  saveProfileDetails(): void {
    this.toastService.success('Profile details modified successfully.');
  }

  terminateSession(id: string): void {
    this.sessions.update(current => current.filter(s => s.id !== id));
    this.toastService.warning('Logged out active session.');
  }
}
