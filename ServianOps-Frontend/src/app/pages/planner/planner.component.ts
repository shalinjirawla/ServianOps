import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { engineers, Job, jobs, statusColors } from '../../core/services/mock-data';

interface Slot { start: number; span: number; job: Job; }
interface Row { engineer: string; slots: Slot[]; }

@Component({
  selector: 'app-planner',
  standalone: true,
  imports: [CommonModule, TopbarComponent, IconComponent],
  templateUrl: './planner.component.html',
  styleUrl: './planner.component.scss',
})
export class PlannerComponent {
  hours = ['08:00', '09:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00'];
  engineers = engineers;
  jobs = jobs;
  statusColors = statusColors;

  scheduled: Row[] = [
    { engineer: 'Owen Hart', slots: [{ start: 1, span: 3, job: jobs[0] }, { start: 6, span: 2, job: jobs[3] }] },
    { engineer: 'Amelia Reed', slots: [{ start: 3, span: 2, job: jobs[1] }, { start: 7, span: 2, job: jobs[4] }] },
    { engineer: 'Jordan Ellis', slots: [{ start: 0, span: 2, job: jobs[2] }, { start: 5, span: 3, job: jobs[6] }] },
    { engineer: 'Priya Shah', slots: [] },
    { engineer: 'Marcus Doyle', slots: [{ start: 2, span: 4, job: jobs[5] }] },
  ];

  get unassigned(): Job[] {
    return this.jobs.filter((j) => j.engineer === '—' || j.status === 'new');
  }

  initials(name: string): string {
    return name.split(' ').map((n) => n[0]).join('');
  }

  slotLeft(start: number): string {
    return `calc(180px + (${start}/10) * (100% - 180px))`;
  }

  slotWidth(span: number): string {
    return `calc((${span}/10) * (100% - 180px) - 6px)`;
  }

  utilWidth(jobCount: number): number {
    return Math.min(100, jobCount * 25);
  }
}
