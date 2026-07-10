import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Tenant {
  id: number;
  companyName: string;
  tenancyName: string;
  planId: number | null;
  isActive: boolean;
  firstName?: string;
  lastName?: string;
  email?: string;
  phone?: string;
  password?: string;
}

@Injectable({
  providedIn: 'root',
})
export class TenantService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/tenants`;

  getTenants(pageNumber = 1, pageSize = 100): Observable<Tenant[]> {
    return this.http.get<Tenant[]>(this.baseUrl, {
      params: { pageNumber, pageSize },
    });
  }

  registerTenant(dto: Partial<Tenant>): Observable<any> {
    return this.http.post<any>(`${environment.apiUrl}/api/auth/register-tenant`, dto);
  }

  updateTenant(id: number, dto: Partial<Tenant>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  deleteTenant(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
