import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { LayoutService } from '../../core/services/layout.service';
import { ToastService } from '../../core/services/toast.service';

interface MetricCard {
  title: string;
  value: string | number;
  icon: string;
  colorClass: string;
  changeText: string;
  isPositive: boolean;
  progress: number; // 0-100 progress value
}

interface ActivityLog {
  id: string;
  actor: string;
  action: string;
  target: string;
  timestamp: string;
  status: 'success' | 'warning' | 'info';
}

interface SeriesPoint {
  date: string;
  value: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  readonly authService = inject(AuthService);
  private readonly layoutService = inject(LayoutService);
  private readonly toastService = inject(ToastService);

  // Dynamic greeting based on active email
  readonly welcomeName = computed(() => {
    const email = this.authService.currentUser()?.email || 'User';
    return email.split('@')[0];
  });

  // Check role context dynamically
  readonly isSuperAdmin = computed(() => this.authService.userRole() === 'SuperAdmin');

  // Simulated metrics and logging lists
  readonly metricCards = signal<MetricCard[]>([]);
  readonly recentActivities = signal<ActivityLog[]>([]);

  // Interactive Graph Series
  readonly activeSeriesIndex = signal<number>(0); // Toggles between primary (0) and secondary (1) datasets

  // Raw Graph Data sets
  readonly superAdminSeries = [
    // Dataset 0: Tenant Registrations
    [
      { date: '14 Jan', value: 110 },
      { date: '15 Jan', value: 118 },
      { date: '16 Jan', value: 114 },
      { date: '17 Jan', value: 120 },
      { date: '18 Jan', value: 125 },
      { date: '19 Jan', value: 122 },
      { date: '20 Jan', value: 124 }
    ],
    // Dataset 1: Monthly Recurring Revenue (in thousands)
    [
      { date: '14 Jan', value: 40 },
      { date: '15 Jan', value: 41 },
      { date: '16 Jan', value: 43 },
      { date: '17 Jan', value: 42 },
      { date: '18 Jan', value: 45 },
      { date: '19 Jan', value: 44 },
      { date: '20 Jan', value: 46 }
    ]
  ];

  readonly tenantAdminSeries = [
    // Dataset 0: Jobs Completed
    [
      { date: '14 Jan', value: 30 },
      { date: '15 Jan', value: 42 },
      { date: '16 Jan', value: 35 },
      { date: '17 Jan', value: 48 },
      { date: '18 Jan', value: 55 },
      { date: '19 Jan', value: 39 },
      { date: '20 Jan', value: 42 }
    ],
    // Dataset 1: Outstanding Invoices Value (in thousands)
    [
      { date: '14 Jan', value: 12 },
      { date: '15 Jan', value: 15 },
      { date: '16 Jan', value: 14 },
      { date: '17 Jan', value: 18 },
      { date: '18 Jan', value: 22 },
      { date: '19 Jan', value: 19 },
      { date: '20 Jan', value: 21 }
    ]
  ];

  // Dynamically resolve active series raw data
  readonly activeSeries = computed<SeriesPoint[]>(() => {
    const isSA = this.isSuperAdmin();
    const idx = this.activeSeriesIndex();
    return isSA ? this.superAdminSeries[idx] : this.tenantAdminSeries[idx];
  });

  // Calculate SVG Coordinates (X from 50 to 550, Y from 30 to 180 based on min/max of the data)
  readonly chartCoordinates = computed(() => {
    const data = this.activeSeries();
    if (data.length === 0) return [];

    const values = data.map(d => d.value);
    const maxVal = Math.max(...values) * 1.1;
    const minVal = Math.min(...values) * 0.9;
    const valRange = maxVal - minVal || 1;

    const width = 500; // 550 - 50
    const height = 150; // 180 - 30
    const stepX = width / (data.length - 1);

    return data.map((point, index) => {
      const x = 50 + index * stepX;
      // Invert Y coordinate since SVG (0,0) is top-left
      const y = 180 - ((point.value - minVal) / valRange) * height;
      return {
        label: point.date,
        value: point.value,
        x: Math.round(x),
        y: Math.round(y)
      };
    });
  });

  // SVG Area shading path
  readonly chartAreaPath = computed(() => {
    const coords = this.chartCoordinates();
    if (coords.length === 0) return '';
    const points = coords.map(c => `${c.x},${c.y}`).join(' L ');
    const firstX = coords[0].x;
    const lastX = coords[coords.length - 1].x;
    return `M ${firstX},200 L ${points} L ${lastX},200 Z`;
  });

  // SVG Line path
  readonly chartLinePath = computed(() => {
    const coords = this.chartCoordinates();
    if (coords.length === 0) return '';
    return 'M ' + coords.map(c => `${c.x},${c.y}`).join(' L ');
  });

  // Chart Titles
  readonly chartTitle = computed(() => {
    if (this.isSuperAdmin()) {
      return this.activeSeriesIndex() === 0 ? 'Tenant Registrations Scale' : 'Monthly Revenue (USD)';
    } else {
      return this.activeSeriesIndex() === 0 ? 'Completed Field Tasks' : 'Invoiced Billings (USD)';
    }
  });

  // Hover state tooltip
  readonly hoveredPointIndex = signal<number | null>(null);

  ngOnInit(): void {
    // Update active layout page title
    this.layoutService.setPageTitle('Dashboard');

    // Populate dashboard statistics based on authenticated role
    if (this.isSuperAdmin()) {
      this.metricCards.set([
        { title: 'Total Registered Tenants', value: 124, icon: 'building-office', colorClass: 'blue', changeText: '+12% this month', isPositive: true, progress: 75 },
        { title: 'Active Plans Offered', value: 3, icon: 'credit-card', colorClass: 'green', changeText: 'Fully active', isPositive: true, progress: 100 },
        { title: 'Monthly Revenue Recurring', value: '$45,890', icon: 'currency-dollar', colorClass: 'indigo', changeText: '+8.4% growth', isPositive: true, progress: 68 },
        { title: 'System Incidents Logs', value: 0, icon: 'shield-check', colorClass: 'red', changeText: 'All systems online', isPositive: true, progress: 100 }
      ]);

      this.recentActivities.set([
        { id: '1', actor: 'System Seeder', action: 'Created new tenant database', target: 'ApexField Solutions', timestamp: '10 mins ago', status: 'success' },
        { id: '2', actor: 'Host admin', action: 'Modified plan parameters', target: 'Enterprise Plan price', timestamp: '1 hour ago', status: 'info' },
        { id: '3', actor: 'Stripe Webhook', action: 'Processed recurring payment', target: 'ApexField Solutions', timestamp: '3 hours ago', status: 'success' },
        { id: '4', actor: 'Database Engine', action: 'Auto backup executed', target: 'PostgreSQL instance', timestamp: '12 hours ago', status: 'info' }
      ]);
    } else {
      // Tenant Administrator Operational Metrics
      this.metricCards.set([
        { title: 'Outstanding Jobs', value: 367, icon: 'wrench-screwdriver', colorClass: 'blue', changeText: '+24 since yesterday', isPositive: true, progress: 82 },
        { title: 'Require Invoicing', value: 84, icon: 'document-text', colorClass: 'orange', changeText: 'Action required', isPositive: false, progress: 42 },
        { title: 'Total Invoiced Value', value: '$50,890', icon: 'banknotes', colorClass: 'green', changeText: '+18% vs last month', isPositive: true, progress: 91 },
        { title: 'Completed Today', value: 42, icon: 'check-circle', colorClass: 'indigo', changeText: '84% target reached', isPositive: true, progress: 84 }
      ]);

      this.recentActivities.set([
        { id: '1', actor: 'John Smith (Admin)', action: 'Dispatched emergency job', target: 'Job #JOB-000184 to Engineer', timestamp: '5 mins ago', status: 'success' },
        { id: '2', actor: 'Mike Johnson (Engineer)', action: 'Marked task completed', target: 'Job #JOB-000182 (AC Repair)', timestamp: '30 mins ago', status: 'success' },
        { id: '3', actor: 'System billing', action: 'Generated custom invoice', target: 'Invoice #INV-2026-904', timestamp: '2 hours ago', status: 'info' },
        { id: '4', actor: 'Tony Davis (Scheduler)', action: 'Updated roster details', target: 'Stock allocation checklist', timestamp: '4 hours ago', status: 'warning' }
      ]);
    }
  }

  setChartSeries(index: number): void {
    this.activeSeriesIndex.set(index);
    this.toastService.info(`Switched visualization chart views.`);
  }
}
