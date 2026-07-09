import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutService } from '../../../core/services/layout.service';

interface CustomerItem {
  id: string;
  name: string;
  code: string;
  email: string;
  phone: string;
  location: string;
  status: 'Active' | 'Inactive';
}

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.scss'
})
export class CustomersComponent implements OnInit {
  private readonly layoutService = inject(LayoutService);

  readonly customers = signal<CustomerItem[]>([]);

  ngOnInit(): void {
    this.layoutService.setPageTitle('Customers Directory');

    // Seed initial mock data matching standard operational context
    this.customers.set([
      { id: '1', name: 'ABC Services Ltd', code: 'CUST-001', email: 'billing@abcservices.com', phone: '+44 7700 900123', location: 'London Office', status: 'Active' },
      { id: '2', name: 'XYZ Industries', code: 'CUST-002', email: 'accounts@xyzind.com', phone: '+44 7700 900124', location: 'Manchester Warehouse', status: 'Active' },
      { id: '3', name: 'Alpha Corporation', code: 'CUST-003', email: 'info@alphacorp.co.uk', phone: '+44 7700 900125', location: 'Birmingham Branch', status: 'Active' },
      { id: '4', name: 'Beta Group UK', code: 'CUST-004', email: 'facility@betagroup.com', phone: '+44 7700 900126', location: 'Bristol Depot', status: 'Inactive' },
      { id: '5', name: 'Omega Logistics', code: 'CUST-005', email: 'maintenance@omega.com', phone: '+44 7700 900127', location: 'Leeds Hub', status: 'Active' }
    ]);
  }
}
