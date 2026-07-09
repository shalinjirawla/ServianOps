import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutService } from '../../../core/services/layout.service';

interface ReportSummary {
  metric: string;
  total: number;
  completed: number;
  pending: number;
  percentage: number;
}

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {
  private readonly layoutService = inject(LayoutService);

  readonly summaries = signal<ReportSummary[]>([]);

  ngOnInit(): void {
    this.layoutService.setPageTitle('Analytics Reports');

    this.summaries.set([
      { metric: 'Reactive Maintenance Jobs', total: 180, completed: 150, pending: 30, percentage: 83.3 },
      { id: '2', metric: 'Planned Preventative Maintenance (PPM)', total: 320, completed: 280, pending: 40, percentage: 87.5 },
      { id: '3', metric: 'Emergency Callouts', total: 45, completed: 42, pending: 3, percentage: 93.3 },
      { id: '4', metric: 'Quoted Works Projects', total: 60, completed: 40, pending: 20, percentage: 66.7 },
      { id: '5', metric: 'Invoicing Compliance Review', total: 240, completed: 210, pending: 30, percentage: 87.5 }
    ] as any);
  }
}
