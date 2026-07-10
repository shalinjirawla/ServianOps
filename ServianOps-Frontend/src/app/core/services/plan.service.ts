import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Plan {
  id: number;
  planName: string;
  maxUsers: number;
  maxProjects: number;
  maxStorageGB: number;
  price: number;
  billingCycle: string;
  isTrialAvailable: boolean;
  trialDays: number;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class PlanService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/plans`;

  getPlans(): Observable<Plan[]> {
    return this.http.get<Plan[]>(this.baseUrl);
  }

  getPlanById(id: number): Observable<Plan> {
    return this.http.get<Plan>(`${this.baseUrl}/${id}`);
  }

  createPlan(plan: Partial<Plan>): Observable<Plan> {
    return this.http.post<Plan>(this.baseUrl, plan);
  }

  updatePlan(id: number, plan: Partial<Plan>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, plan);
  }

  deletePlan(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
