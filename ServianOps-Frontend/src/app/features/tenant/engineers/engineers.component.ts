import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutService } from '../../../core/services/layout.service';

interface EngineerItem {
  id: string;
  name: string;
  email: string;
  phone: string;
  status: 'Active' | 'Inactive' | 'On Leave';
}

@Component({
  selector: 'app-engineers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './engineers.component.html',
  styleUrl: './engineers.component.scss'
})
export class EngineersComponent implements OnInit {
  private readonly layoutService = inject(LayoutService);

  readonly engineers = signal<EngineerItem[]>([]);

  ngOnInit(): void {
    this.layoutService.setPageTitle('Engineers Directory');

    this.engineers.set([
      { id: '1', name: 'John Smith', email: 'john.smith@servianops.com', phone: '+44 7700 900123', status: 'Active' },
      { id: '2', name: 'Mike Johnson', email: 'mike.johnson@servianops.com', phone: '+44 7700 900124', status: 'Active' },
      { id: '3', name: 'David Brown', email: 'david.brown@servianops.com', phone: '+44 7700 900125', status: 'Active' },
      { id: '4', name: 'James Wilson', email: 'james.wilson@servianops.com', phone: '+44 7700 900126', status: 'On Leave' },
      { id: '5', name: 'Tony Davis', email: 'tony.davis@servianops.com', phone: '+44 7700 900127', status: 'Active' }
    ]);
  }
}
