import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface SiteContact {
  id: number;
  firstName: string;
  lastName: string;
  mobileNumber: string;
  email: string;
  isActive: boolean;
}

export interface SiteListDto {
  id: number;
  customer: { id: number; name: string };
  siteName: string;
  companyName: string;
  mobileNumber: string;
  accountManagerName?: string;
  primaryContactName?: string;
  primaryContactMobile?: string;
  creationTime: string;
  isActive: boolean;
}

export interface SiteDetailDto {
  id: number;
  customerId: number;
  customer: { id: number; name: string };
  siteName: string;
  companyName: string;
  area: string;
  city: string;
  countryOrState: string;
  postCode: string;
  mobileNumber: string;
  accessDetails?: string;
  parkingInformation?: string;
  keysOrCode?: string;
  siteNotes?: string;
  accountManagerId?: number | null;
  accountManagerName?: string;
  creationTime: string;
  isActive: boolean;
  contacts: SiteContact[];
}

@Injectable({
  providedIn: 'root',
})
export class SiteService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/sites`;

  getSites(pageNumber = 1, pageSize = 100): Observable<SiteListDto[]> {
    return this.http.get<SiteListDto[]>(this.baseUrl, {
      params: { pageNumber: pageNumber.toString(), pageSize: pageSize.toString() },
    });
  }

  getSiteById(id: number): Observable<SiteDetailDto> {
    return this.http.get<SiteDetailDto>(`${this.baseUrl}/${id}`);
  }

  createSite(dto: any): Observable<SiteDetailDto> {
    return this.http.post<SiteDetailDto>(this.baseUrl, dto);
  }

  updateSite(id: number, dto: any): Observable<SiteDetailDto> {
    return this.http.put<SiteDetailDto>(`${this.baseUrl}/${id}`, dto);
  }

  deleteSite(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
