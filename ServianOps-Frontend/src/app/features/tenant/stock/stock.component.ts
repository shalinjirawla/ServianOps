import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutService } from '../../../core/services/layout.service';

interface StockItem {
  id: string;
  partNo: string;
  name: string;
  category: string;
  quantity: number;
  minAlertLevel: number;
  status: 'In Stock' | 'Low Stock' | 'Out of Stock';
}

@Component({
  selector: 'app-stock',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stock.component.html',
  styleUrl: './stock.component.scss'
})
export class StockComponent implements OnInit {
  private readonly layoutService = inject(LayoutService);

  readonly stock = signal<StockItem[]>([]);

  ngOnInit(): void {
    this.layoutService.setPageTitle('Stock & Inventory');

    this.stock.set([
      { id: '1', partNo: 'PART-901', name: 'Smart Thermostat V3', category: 'HVAC Controls', quantity: 24, minAlertLevel: 5, status: 'In Stock' },
      { id: '2', partNo: 'PART-902', name: 'AC Copper Piping (50m)', category: 'Piping', quantity: 3, minAlertLevel: 5, status: 'Low Stock' },
      { id: '3', partNo: 'PART-903', name: '12V Relay Switch Pack', category: 'Electrical', quantity: 45, minAlertLevel: 10, status: 'In Stock' },
      { id: '4', partNo: 'PART-904', name: 'Replacement Fan Blade 18"', category: 'HVAC Spares', quantity: 0, minAlertLevel: 2, status: 'Out of Stock' },
      { id: '5', partNo: 'PART-905', name: 'Compressor Unit 2.5HP', category: 'HVAC Units', quantity: 8, minAlertLevel: 2, status: 'In Stock' }
    ]);
  }
}
