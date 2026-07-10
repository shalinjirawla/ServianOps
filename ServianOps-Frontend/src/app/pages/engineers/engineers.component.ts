import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { engineers as mockEngineers } from '../../core/services/mock-data';

const availTone: Record<string, string> = {
  'Available': 'bg-success/15 text-success border-success/30',
  'On Job': 'bg-warning/15 text-warning-foreground border-warning/30',
  'Travelling': 'bg-info/15 text-info border-info/30',
  'Off shift': 'bg-muted text-muted-foreground border-border',
};

@Component({
  selector: 'app-engineers',
  standalone: true,
  imports: [CommonModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './engineers.component.html',
  styleUrl: './engineers.component.scss',
})
export class EngineersComponent implements OnInit {
  rawEngineers = mockEngineers;
  availTone = availTone;

  // Pagination State
  currentPage = 1;
  pageSize = 6;
  mathMin = Math.min;

  // Sorting State
  sortField = 'name';
  sortAsc = true;

  // Filter State
  searchQuery = '';

  ngOnInit() {}

  get filteredAndSortedEngineers() {
    let list = [...this.rawEngineers];

    // 1. Filtering
    const q = this.searchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(e =>
        (e.name && e.name.toLowerCase().includes(q)) ||
        (e.trade && e.trade.toLowerCase().includes(q)) ||
        (e.region && e.region.toLowerCase().includes(q)) ||
        (e.availability && e.availability.toLowerCase().includes(q))
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

  get engineers() {
    const list = this.filteredAndSortedEngineers;
    const start = (this.currentPage - 1) * this.pageSize;
    return list.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredAndSortedEngineers.length / this.pageSize) || 1;
  }

  setSort(field: string) {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = true;
    }
  }

  initials(name: string): string {
    return name ? name.split(' ').map((n) => n[0]).join('') : '';
  }
}
