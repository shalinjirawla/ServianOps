import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutService } from '../../../core/services/layout.service';

interface InvoiceItem {
  id: string;
  invoiceNo: string;
  customer: string;
  amount: number;
  issueDate: string;
  dueDate: string;
  status: 'Paid' | 'Unpaid' | 'Overdue';
}

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.scss'
})
export class InvoicesComponent implements OnInit {
  private readonly layoutService = inject(LayoutService);

  readonly invoices = signal<InvoiceItem[]>([]);

  ngOnInit(): void {
    this.layoutService.setPageTitle('Invoices & Billing');

    this.invoices.set([
      { id: '1', invoiceNo: 'INV-2026-001', customer: 'ABC Services Ltd', amount: 1250.00, issueDate: '05/01/2026', dueDate: '05/02/2026', status: 'Paid' },
      { id: '2', invoiceNo: 'INV-2026-002', customer: 'XYZ Industries', amount: 5600.00, issueDate: '10/01/2026', dueDate: '10/02/2026', status: 'Unpaid' },
      { id: '3', invoiceNo: 'INV-2026-003', customer: 'Alpha Corporation', amount: 890.50, issueDate: '12/01/2026', dueDate: '12/02/2026', status: 'Paid' },
      { id: '4', invoiceNo: 'INV-2026-004', customer: 'Beta Group UK', amount: 3200.00, issueDate: '15/01/2026', dueDate: '15/02/2026', status: 'Overdue' },
      { id: '5', invoiceNo: 'INV-2026-005', customer: 'Omega Logistics', amount: 1540.00, issueDate: '18/01/2026', dueDate: '18/02/2026', status: 'Unpaid' }
    ]);
  }

  getTotalAmount(): number {
    return this.invoices().reduce((acc, inv) => acc + inv.amount, 0);
  }

  getPaidAmount(): number {
    return this.invoices()
      .filter(i => i.status === 'Paid')
      .reduce((acc, inv) => acc + inv.amount, 0);
  }

  getUnpaidAmount(): number {
    return this.invoices()
      .filter(i => i.status !== 'Paid')
      .reduce((acc, inv) => acc + inv.amount, 0);
  }
}
