import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface User {
  id: number;
  tenantId: number | null;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  profileImage?: string;
  isActive: boolean;
  password?: string;
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/users`;

  getUsers(pageNumber = 1, pageSize = 100, tenantId?: number | null): Observable<User[]> {
    const params: any = { pageNumber, pageSize };
    if (tenantId) {
      params.tenantId = tenantId.toString();
    }
    return this.http.get<User[]>(this.baseUrl, { params });
  }

  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/${id}`);
  }

  createUser(user: Partial<User>, tenantId?: number | null): Observable<User> {
    const params: any = {};
    if (tenantId) {
      params.tenantId = tenantId.toString();
    }
    return this.http.post<User>(this.baseUrl, user, { params });
  }

  updateUser(id: number, user: Partial<User>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, user);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  toggleUserStatus(id: number): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/toggle-status`, {});
  }
}
