import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { activityFeed, Job, jobs, statusColors } from '../../core/services/mock-data';

interface TrendPoint { d: string; jobs: number; done: number; }
interface TradePoint { t: string; v: number; }
interface Stat { icon: string; label: string; value: string; delta: string; tone: 'up' | 'down'; }
interface Sla { label: string; val: number; }

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, TopbarComponent, IconComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  jobs: Job[] = jobs.slice(0, 5);
  statusColors = statusColors;
  activityFeed = activityFeed;

  stats: Stat[] = [
    { icon: 'briefcase', label: 'Open jobs', value: '127', delta: '+12% vs last week', tone: 'up' },
    { icon: 'clock', label: 'SLA at risk', value: '8', delta: '-3 vs yesterday', tone: 'up' },
    { icon: 'check-circle-2', label: 'Completed today', value: '34', delta: '+21% vs avg', tone: 'up' },
    { icon: 'alert-triangle', label: 'Quotes pending', value: '19', delta: '+4 this week', tone: 'down' },
  ];

  sla: Sla[] = [
    { label: 'P1 — 4hr response', val: 96 },
    { label: 'P2 — 24hr response', val: 89 },
    { label: 'P3 — 3 day response', val: 94 },
    { label: 'P4 — 7 day response', val: 98 },
  ];

  trend: TrendPoint[] = [
    { d: 'Mon', jobs: 24, done: 18 }, { d: 'Tue', jobs: 31, done: 22 },
    { d: 'Wed', jobs: 28, done: 25 }, { d: 'Thu', jobs: 36, done: 27 },
    { d: 'Fri', jobs: 42, done: 33 }, { d: 'Sat', jobs: 18, done: 15 },
    { d: 'Sun', jobs: 12, done: 11 },
  ];

  tradeMix: TradePoint[] = [
    { t: 'HVAC', v: 34 }, { t: 'Electrical', v: 28 }, { t: 'Plumbing', v: 22 },
    { t: 'Glazing', v: 12 }, { t: 'Security', v: 18 }, { t: 'Fabric', v: 9 },
  ];

  // ---- Chart geometry (plain SVG, no chart library) ----
  chartW = 640;
  chartH = 220;
  padL = 28;
  padB = 20;
  padT = 10;

  private scaleX(i: number, count: number): number {
    const usable = this.chartW - this.padL - 8;
    return this.padL + (usable * i) / (count - 1);
  }

  private scaleY(v: number, max: number): number {
    const usable = this.chartH - this.padT - this.padB;
    return this.padT + usable - (usable * v) / max;
  }

  get trendMax(): number {
    return Math.max(...this.trend.map((t) => Math.max(t.jobs, t.done))) * 1.15;
  }

  get jobsLinePoints(): string {
    return this.trend.map((p, i) => `${this.scaleX(i, this.trend.length)},${this.scaleY(p.jobs, this.trendMax)}`).join(' ');
  }

  get doneLinePoints(): string {
    return this.trend.map((p, i) => `${this.scaleX(i, this.trend.length)},${this.scaleY(p.done, this.trendMax)}`).join(' ');
  }

  trendDotsFor(key: 'jobs' | 'done') {
    return this.trend.map((p, i) => ({
      x: this.scaleX(i, this.trend.length),
      y: this.scaleY(p[key], this.trendMax),
    }));
  }

  get tradeMax(): number {
    return Math.max(...this.tradeMix.map((t) => t.v)) * 1.1;
  }

  tradeBarWidth(v: number): number {
    return (v / this.tradeMax) * 100;
  }
}
