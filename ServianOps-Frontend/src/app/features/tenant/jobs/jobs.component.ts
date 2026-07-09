import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutService } from '../../../core/services/layout.service';

interface JobItem {
  id: string;
  jobNo: string;
  customer: string;
  site: string;
  jobType: string;
  status: 'In Progress' | 'Outstanding' | 'Completed' | 'Pending';
  engineer: string;
  date: string;
}

@Component({
  selector: 'app-jobs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './jobs.component.html',
  styleUrl: './jobs.component.scss'
})
export class JobsComponent implements OnInit {
  private readonly layoutService = inject(LayoutService);

  readonly jobs = signal<JobItem[]>([]);
  readonly activeStatusTab = signal<string>('All');

  ngOnInit(): void {
    this.layoutService.setPageTitle('Jobs Management');

    this.jobs.set([
      { id: '1', jobNo: 'JOB-000184', customer: 'ABC Services', site: 'Head Office', jobType: 'Maintenance', status: 'In Progress', engineer: 'John Smith', date: '20/01/2026' },
      { id: '2', jobNo: 'JOB-000183', customer: 'XYZ Limited', site: 'Main Site', jobType: 'Reactive', status: 'Outstanding', engineer: 'Mike Johnson', date: '20/01/2026' },
      { id: '3', jobNo: 'JOB-000182', customer: 'Alpha Corp', site: 'Branch Office', jobType: 'Installation', status: 'Completed', engineer: 'David Brown', date: '19/01/2026' },
      { id: '4', jobNo: 'JOB-000181', customer: 'Beta Group', site: 'Site 2', jobType: 'Maintenance', status: 'In Progress', engineer: 'James Wilson', date: '19/01/2026' },
      { id: '5', jobNo: 'JOB-000180', customer: 'Omega Ltd', site: 'Warehouse', jobType: 'PPM', status: 'Outstanding', engineer: 'Tony Davis', date: '18/01/2026' }
    ]);
  }

  setTab(tab: string): void {
    this.activeStatusTab.set(tab);
  }

  getFilteredJobs(): JobItem[] {
    const tab = this.activeStatusTab();
    if (tab === 'All') return this.jobs();
    return this.jobs().filter(j => j.status === tab);
  }

  getStatusCount(status: string): number {
    if (status === 'All') return this.jobs().length;
    return this.jobs().filter(j => j.status === status).length;
  }
}
