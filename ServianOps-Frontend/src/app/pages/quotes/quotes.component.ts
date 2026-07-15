import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { quotes as mockQuotes } from '../../core/services/mock-data';

const tone: Record<string, string> = {
  sent: 'bg-info/15 text-info border-info/30',
  approved: 'bg-success/15 text-success border-success/30',
  declined: 'bg-destructive/15 text-destructive border-destructive/30',
};

@Component({
  selector: 'app-quotes',
  standalone: true,
  imports: [CommonModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './quotes.component.html',
  styleUrl: './quotes.component.scss',
})
export class QuotesComponent implements OnInit {
  rawQuotes = mockQuotes;
  tone = tone;

  summary = [
    { l: 'Draft', v: 4, tone: 'text-muted-foreground' },
    { l: 'Sent', v: 12, tone: 'text-info' },
    { l: 'Approved', v: 8, tone: 'text-success' },
    { l: 'Declined', v: 3, tone: 'text-destructive' },
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

  ngOnInit() { }

  get filteredAndSortedQuotes() {
    let list = [...this.rawQuotes];

    // 1. Filtering
    const q = this.searchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(item =>
        (item.id && item.id.toLowerCase().includes(q)) ||
        (item.job && item.job.toLowerCase().includes(q)) ||
        (item.client && item.client.toLowerCase().includes(q)) ||
        (item.scope && item.scope.toLowerCase().includes(q)) ||
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

  get quotes() {
    const list = this.filteredAndSortedQuotes;
    const start = (this.currentPage - 1) * this.pageSize;
    return list.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredAndSortedQuotes.length / this.pageSize) || 1;
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
