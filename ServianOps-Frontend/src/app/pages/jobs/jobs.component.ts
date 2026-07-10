import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { ToastService } from '../../shared/toast/toast.service';
import { JobService, JobListDto, PriorityEnum } from '../../core/services/job.service';
import { CustomerService } from '../../core/services/customer.service';
import { SiteService } from '../../core/services/site.service';
import { TradeService } from '../../core/services/trade.service';

const priorityTone: Record<string, string> = {
  P1: 'bg-destructive text-destructive-foreground',
  P2: 'bg-warning text-warning-foreground',
  P3: 'bg-info text-info-foreground',
  P4: 'bg-muted text-muted-foreground',
};

const statusColors: Record<string, string> = {
  new: 'bg-info/10 text-info border-info/20',
  assigned: 'bg-primary/10 text-primary border-primary/20',
  'in-progress': 'bg-warning/10 text-warning-foreground border-warning/20',
  complete: 'bg-success/10 text-success border-success/20',
};

@Component({
  selector: 'app-jobs',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TopbarComponent, IconComponent],
  templateUrl: './jobs.component.html',
  styleUrl: './jobs.component.scss',
})
export class JobsComponent implements OnInit {
  private jobService = inject(JobService);
  private customerService = inject(CustomerService);
  private siteService = inject(SiteService);
  private tradeService = inject(TradeService);
  private toast = inject(ToastService);
  private fb = inject(FormBuilder);

  rawJobs: JobListDto[] = [];
  customers: { id: number; name: string }[] = [];
  sites: { id: number; siteName: string; customerId: number }[] = [];
  trades: { id: number; name: string }[] = [];

  priorityTone = priorityTone;
  statusColors = statusColors;
  activeTab = 'all';

  // Pagination State
  currentPage = 1;
  pageSize = 10;
  mathMin = Math.min;

  // Sorting State
  sortField = 'id';
  sortAsc = false; // default newest first

  // Filter State
  searchQuery = '';

  // Job Creation Drawer Form
  showDrawer = false;
  submitted = false;
  jobForm!: FormGroup;

  ngOnInit() {
    this.initForm();
    this.loadJobs();
    this.loadDropdownData();
  }

  initForm() {
    this.jobForm = this.fb.group({
      customerId: ['', [Validators.required]],
      siteId: ['', [Validators.required]],
      tradeId: ['', [Validators.required]],
      description: ['', [Validators.required, Validators.minLength(5)]],
      priority: ['2', [Validators.required]], // default Medium (2)
      poNumber: [''],
      budget: [0, [Validators.min(0)]],
      nte: [0, [Validators.min(0)]]
    });

    // When customer changes, load their sites
    this.jobForm.get('customerId')?.valueChanges.subscribe((customerId) => {
      if (customerId) {
        this.jobForm.patchValue({ siteId: '' });
      }
    });
  }

  loadJobs() {
    this.jobService.getJobs(1, 200).subscribe({
      next: (data) => {
        this.rawJobs = data;
      },
      error: () => {
        this.toast.error('Failed to load jobs from backend');
      }
    });
  }

  loadDropdownData() {
    this.customerService.getCustomersDropdown().subscribe({
      next: (data) => {
        this.customers = data;
      }
    });

    this.siteService.getSites(1, 200).subscribe({
      next: (data) => {
        this.sites = data.map(s => ({
          id: s.id,
          siteName: s.siteName,
          customerId: s.customer?.id
        }));
      }
    });

    this.tradeService.getTrades(1, 200).subscribe({
      next: (data) => {
        this.trades = data;
        if (data.length === 0) {
          // seed a default trade if none exists
          this.tradeService.createTrade('General').subscribe({
            next: (newTrade) => {
              this.trades = [newTrade];
            }
          });
        }
      }
    });
  }

  get filteredSites() {
    const custId = Number(this.jobForm.get('customerId')?.value);
    if (!custId) return [];
    return this.sites.filter(s => s.customerId === custId);
  }

  setTab(t: string) {
    this.activeTab = t;
    this.currentPage = 1;
  }

  get filteredAndSortedJobs() {
    let list = this.rawJobs.map(j => {
      // Map PriorityEnum
      let priorityStr = 'P4';
      if (j.priority === 4) priorityStr = 'P1';
      else if (j.priority === 3) priorityStr = 'P2';
      else if (j.priority === 2) priorityStr = 'P3';

      return {
        id: j.jobNumber || `JB-${j.id}`,
        rawId: j.id,
        title: j.description || 'No Description',
        client: j.customer?.name || 'Unknown Client',
        site: j.site?.siteName || 'Unknown Site',
        trade: j.trade?.name || 'General',
        priority: priorityStr,
        status: 'new', // default status
        engineer: 'Unassigned',
        scheduled: 'Unscheduled',
        po: 'N/A',
        description: j.description || ''
      };
    });

    // 1. Tab Filtering
    if (this.activeTab === 'unassigned') {
      list = list.filter(j => j.engineer === 'Unassigned' || j.status === 'new');
    } else if (this.activeTab === 'onsite') {
      list = list.filter(j => j.status === 'in-progress');
    } else if (this.activeTab === 'sla') {
      list = list.filter(j => j.priority === 'P1');
    }

    // 2. Search query filtering
    const q = this.searchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(j =>
        j.id.toLowerCase().includes(q) ||
        j.title.toLowerCase().includes(q) ||
        j.client.toLowerCase().includes(q) ||
        j.site.toLowerCase().includes(q) ||
        j.trade.toLowerCase().includes(q)
      );
    }

    // 3. Sorting
    list.sort((a, b) => {
      let valA: any = '';
      let valB: any = '';

      if (this.sortField === 'id') {
        valA = a.rawId;
        valB = b.rawId;
      } else if (this.sortField === 'client') {
        valA = a.client;
        valB = b.client;
      } else if (this.sortField === 'priority') {
        valA = a.priority;
        valB = b.priority;
      } else {
        valA = (a as any)[this.sortField] || '';
        valB = (b as any)[this.sortField] || '';
      }

      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();

      if (valA < valB) return this.sortAsc ? -1 : 1;
      if (valA > valB) return this.sortAsc ? 1 : -1;
      return 0;
    });

    return list;
  }

  get jobs() {
    const list = this.filteredAndSortedJobs;
    const start = (this.currentPage - 1) * this.pageSize;
    return list.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredAndSortedJobs.length / this.pageSize) || 1;
  }

  setSort(field: string) {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = true;
    }
  }

  openDrawer() {
    this.submitted = false;
    this.jobForm.reset({
      priority: '2',
      budget: 0,
      nte: 0
    });
    this.showDrawer = true;
  }

  closeDrawer() {
    this.submitted = false;
    this.showDrawer = false;
    this.jobForm.reset();
  }

  saveJob() {
    this.submitted = true;
    if (this.jobForm.invalid) {
      this.jobForm.markAllAsTouched();
      return;
    }

    const val = this.jobForm.value;
    const formData = new FormData();
    formData.append('customerId', val.customerId.toString());
    formData.append('siteId', val.siteId.toString());
    formData.append('tradeId', val.tradeId.toString());
    formData.append('description', val.description);
    formData.append('priority', val.priority.toString());
    formData.append('poNumber', val.poNumber || '');
    formData.append('budget', (val.budget || 0).toString());
    formData.append('nte', (val.nte || 0).toString());

    this.jobService.createJob(formData).subscribe({
      next: () => {
        this.toast.success('Job logged successfully');
        this.loadJobs();
        this.closeDrawer();
      },
      error: () => {
        this.toast.error('Failed to raise new job request');
      }
    });
  }
}
