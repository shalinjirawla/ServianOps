import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export enum PriorityEnum {
  Low = 1,
  Medium = 2,
  High = 3,
  Emergency = 4
}

export interface JobAttachment {
  id: number;
  fileName: string;
  filePath: string;
}

export interface JobListDto {
  id: number;
  jobNumber: string;
  description: string;
  customer: { id: number; name: string };
  site: { id: number; siteName: string; companyName: string };
  trade: { id: number; name: string };
  priority: PriorityEnum;
  creationTime: string;
}

export interface JobDetailDto {
  id: number;
  jobNumber: string;
  customerId: number;
  customer: { id: number; name: string };
  siteId: number;
  site: { id: number; siteName: string; companyName: string };
  tradeId: number;
  trade: { id: number; name: string };
  description: string;
  priority: PriorityEnum;
  poNumber: string;
  budget: number;
  nte: number;
  creationTime: string;
  isActive: boolean;
  attachments: JobAttachment[];
}

@Injectable({
  providedIn: 'root',
})
export class JobService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/jobs`;

  getJobs(pageNumber = 1, pageSize = 100, search?: string): Observable<JobListDto[]> {
    const params: any = { pageNumber: pageNumber.toString(), pageSize: pageSize.toString() };
    if (search) {
      params.search = search;
    }
    return this.http.get<JobListDto[]>(this.baseUrl, { params });
  }

  getJobById(id: number): Observable<JobDetailDto> {
    return this.http.get<JobDetailDto>(`${this.baseUrl}/${id}`);
  }

  createJob(formData: FormData): Observable<JobDetailDto> {
    return this.http.post<JobDetailDto>(this.baseUrl, formData);
  }

  updateJob(id: number, formData: FormData): Observable<JobDetailDto> {
    return this.http.put<JobDetailDto>(`${this.baseUrl}/${id}`, formData);
  }

  deleteJob(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  deleteAttachment(jobId: number, attachmentId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${jobId}/attachments/${attachmentId}`);
  }
}
