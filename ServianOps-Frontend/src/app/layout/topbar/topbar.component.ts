import { Component, inject, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../../shared/icon/icon.component';
import { AuthService } from '../../core/services/auth.service';
import { SidebarService } from '../sidebar/sidebar.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
})
export class TopbarComponent implements OnInit {
  @Input() title = '';
  @Input() subtitle?: string;

  name = 'Admin';
  role = 'Operations Admin';
  tenancyName = '';

  constructor(
    private auth: AuthService,
    public sidebarSvc: SidebarService
  ) {}

  ngOnInit() {
    // const s = this.auth.getSession();
    // if (s) {
    //   this.name = s.name;
    //   if (s.role) {
    //     this.role = s.role === 'SuperAdmin' ? 'Super Admin' : 'Admin';
    //   }
    //   if (s.tenancyName) {
    //     this.tenancyName = s.tenancyName;
    //   }
    // }
  }

  handleToggle() {
    if (window.innerWidth < 1024) {
      this.sidebarSvc.toggleMobile();
    } else {
      this.sidebarSvc.toggle();
    }
  }

  get initials(): string {
    return this.name.slice(0, 2).toUpperCase();
  }
}
