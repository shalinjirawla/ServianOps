import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface CustomerType {
  id: number;
  name: string;
}

export interface CustomerContact {
  id: number;
  firstName: string;
  lastName: string;
  mobileNumber: string;
  email: string;
  isActive: boolean;
}

export interface Customer {
  id: number;
  name: string;
  companyName: string;
  area: string;
  city: string;
  countryOrState: string;
  postCode: string;
  mobileNumber: string;
  accountNumber: string;
  paymentTerms: number;
  isVatRegistered: boolean;
  vatNumber: string;
  isPORequired: boolean;
  customerTypeId: number;
  customerType?: CustomerType;
  accountManagerId?: number | null;
  accountManagerName?: string;
  sellingRateId?: number | null;
  creationTime: string;
  isActive: boolean;
  contacts?: CustomerContact[];
}

export interface CustomerListDto {
  id: number;
  name: string;
  companyName: string;
  mobileNumber: string;
  customerType?: CustomerType;
  accountManagerName?: string;
  primaryContactName?: string;
  primaryContactMobile?: string;
  creationTime: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/customers`;
  private readonly typeUrl = `${environment.apiUrl}/api/customertypes`;

  getCustomers(pageNumber = 1, pageSize = 100): Observable<CustomerListDto[]> {
    return this.http.get<CustomerListDto[]>(this.baseUrl, {
      params: { pageNumber: pageNumber.toString(), pageSize: pageSize.toString() },
    });
  }

  getCustomerById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.baseUrl}/${id}`);
  }

  getCustomersDropdown(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/dropdown`);
  }

  getCustomerSites(id: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/${id}/sites`);
  }

  createCustomer(dto: any): Observable<Customer> {
    return this.http.post<Customer>(this.baseUrl, dto);
  }

  updateCustomer(id: number, dto: any): Observable<Customer> {
    return this.http.put<Customer>(`${this.baseUrl}/${id}`, dto);
  }

  deleteCustomer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  // Customer Types management helper
  getCustomerTypes(pageNumber = 1, pageSize = 100): Observable<CustomerType[]> {
    return this.http.get<CustomerType[]>(this.typeUrl, {
      params: { pageNumber: pageNumber.toString(), pageSize: pageSize.toString() },
    });
  }

  createCustomerType(name: string): Observable<CustomerType> {
    return this.http.post<CustomerType>(this.typeUrl, { name });
  }
}
