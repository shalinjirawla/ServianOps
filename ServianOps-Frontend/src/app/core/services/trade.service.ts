import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Trade {
  id: number;
  name: string;
}

@Injectable({
  providedIn: 'root',
})
export class TradeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/trades`;

  getTrades(pageNumber = 1, pageSize = 100, search?: string): Observable<Trade[]> {
    const params: any = { pageNumber: pageNumber.toString(), pageSize: pageSize.toString() };
    if (search) {
      params.search = search;
    }
    return this.http.get<Trade[]>(this.baseUrl, { params });
  }

  getTradeById(id: number): Observable<Trade> {
    return this.http.get<Trade>(`${this.baseUrl}/${id}`);
  }

  createTrade(name: string): Observable<Trade> {
    return this.http.post<Trade>(this.baseUrl, { name });
  }

  updateTrade(id: number, name: string): Observable<Trade> {
    return this.http.put<Trade>(`${this.baseUrl}/${id}`, { name });
  }

  deleteTrade(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
