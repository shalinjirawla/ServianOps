import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { invoices as mockInvoices } from '../../core/services/mock-data';

const tone: Record<string, string> = {
  paid: 'bg-success/15 text-success border-success/30',
  sent: 'bg-info/15 text-info border-info/30',
  overdue: 'bg-destructive/15 text-destructive border-destructive/30',
  draft: 'bg-muted text-muted-foreground border-border',
};

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.scss',
})
export class InvoicesComponent implements OnInit {
  rawInvoices = mockInvoices;
  tone = tone;

  summary = [
    { l: 'Outstanding', v: '£24,820', sub: '12 invoices', tone: '' },
    { l: 'Overdue', v: '£6,420', sub: '1 invoice', tone: 'text-destructive' },
    { l: 'Paid this month', v: '£38,140', sub: '22 invoices', tone: 'text-success' },
    { l: 'Draft', v: '£4,120', sub: '3 invoices', tone: 'text-muted-foreground' },
  ];

  // Pagination State
  currentPage = 1;
  pageSize = 5;
  mathMin = Math.min;

  // Sorting State
  sortField = 'id';
  sortAsc = true;

  // Filter State
  searchQuery = '';

  ngOnInit() {}

  get filteredAndSortedInvoices() {
    let list = [...this.rawInvoices];

    // 1. Filtering
    const q = this.searchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(item =>
        (item.id && item.id.toLowerCase().includes(q)) ||
        (item.client && item.client.toLowerCase().includes(q)) ||
        (item.job && item.job.toLowerCase().includes(q)) ||
        (item.status && item.status.toLowerCase().includes(q))
      );
    }

    // 2. Sorting
    list.sort((a, b) => {
      let valA: any = (a as any)[this.sortField] || '';
      let valB: any = (b as any)[this.sortField] || '';

      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();

      if (valA < valB) return this.sortAsc ? -1 : 1;
      if (valA > valB) return this.sortAsc ? 1 : -1;
      return 0;
    });

    return list;
  }

  get invoices() {
    const list = this.filteredAndSortedInvoices;
    const start = (this.currentPage - 1) * this.pageSize;
    return list.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredAndSortedInvoices.length / this.pageSize) || 1;
  }

  setSort(field: string) {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = true;
    }
  }

  fmt(v: number): string {
    return v.toLocaleString('en-GB');
  }
}
