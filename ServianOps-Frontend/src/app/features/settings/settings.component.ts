import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LayoutService } from '../../core/services/layout.service';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';
import { ToastService } from '../../core/services/toast.service';

type SettingsTab = 'company' | 'preferences' | 'security' | 'integrations';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class SettingsComponent implements OnInit {
  private readonly layoutService = inject(LayoutService);
  readonly authService = inject(AuthService);
  readonly themeService = inject(ThemeService);
  private readonly toastService = inject(ToastService);

  // Tab management signal
  readonly activeTab = signal<SettingsTab>('company');

  // Security variables
  show2FaQr = signal(false);
  oldPassword = '';
  newPassword = '';
  confirmPassword = '';

  // Preferences variables
  language = 'English (UK)';
  landingPage = '/dashboard';
  itemsPerPage = 10;
  pushNotifications = true;
  emailDigest = true;

  // Integrations variables
  stripeConnected = signal(true);
  slackConnected = signal(false);
  apiKeyGenerated = signal(false);
  generatedApiKey = signal('');

  ngOnInit(): void {
    this.layoutService.setPageTitle('System Settings');
  }

  setTab(tab: SettingsTab): void {
    this.activeTab.set(tab);
  }

  saveCompanySettings(): void {
    this.toastService.success('Company specifications updated successfully.');
  }

  savePreferences(): void {
    this.toastService.success('Workspace preferences saved successfully.');
  }

  updatePassword(): void {
    if (!this.oldPassword || !this.newPassword || !this.confirmPassword) {
      this.toastService.error('Please fill in all password fields.');
      return;
    }
    if (this.newPassword !== this.confirmPassword) {
      this.toastService.error('New passwords do not match.');
      return;
    }
    this.toastService.success('Password credentials modified successfully.');
    this.oldPassword = '';
    this.newPassword = '';
    this.confirmPassword = '';
  }

  toggle2Fa(): void {
    this.show2FaQr.update(val => !val);
    if (this.show2FaQr()) {
      this.toastService.info('Two-Factor Authentication setup generated.');
    } else {
      this.toastService.warning('Two-Factor Authentication configuration closed.');
    }
  }

  toggleIntegration(provider: 'Stripe' | 'Slack'): void {
    if (provider === 'Stripe') {
      this.stripeConnected.update(val => !val);
      const msg = this.stripeConnected() ? 'Stripe Gateway integration activated.' : 'Stripe billing integrations offline.';
      this.toastService.show(this.stripeConnected() ? 'success' : 'warning', msg);
    } else {
      this.slackConnected.update(val => !val);
      const msg = this.slackConnected() ? 'Slack notifications bridge enabled.' : 'Slack webhook dispatcher closed.';
      this.toastService.show(this.slackConnected() ? 'success' : 'warning', msg);
    }
  }

  generateApiKey(): void {
    this.apiKeyGenerated.set(true);
    const key = 'so_live_' + Math.random().toString(36).substring(2, 15) + Math.random().toString(36).substring(2, 15);
    this.generatedApiKey.set(key);
    this.toastService.success('Generated live API access key.');
  }
}
