import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { ToastService } from '../../shared/toast/toast.service';
import { TenantService, Tenant } from '../../core/services/tenant.service';
import { PlanService } from '../../core/services/plan.service';
import { ConfirmationModalService } from '../../shared/confirmation-modal/confirmation-modal.service';

@Component({
  selector: 'app-tenants',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './tenants.component.html',
  styleUrl: './tenants.component.scss',
})
export class TenantsComponent implements OnInit {
  searchQuery = '';
  rawTenants: Tenant[] = [];
  plans: any[] = [];

  tenantForm!: FormGroup;

  constructor(
    private tenantService: TenantService,
    private planService: PlanService,
    private toast: ToastService,
    private confirmSvc: ConfirmationModalService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.loadPlans();
    this.loadTenants();
    this.initForm();
  }

  initForm() {
    this.tenantForm = this.fb.group({
      companyName: ['', [Validators.required]],
      tenancyName: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9-]+$'), Validators.minLength(2)]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern('^(\\+?[0-9\\s\\-\\(\\)]{7,20})?$')]],
      planId: [1, [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  loadPlans() {
    this.planService.getPlans().subscribe({
      next: (data) => {
        this.plans = data.map((p) => ({
          id: p.id,
          name: p.planName,
          price: `£${p.price / 100}/mo`
        }));
      },
      error: (err) => {
        this.plans = [
          { id: 1, name: 'Basic Plan', price: '£10/mo' },
          { id: 2, name: 'Premium Plan', price: '£50/mo' },
          { id: 3, name: 'Enterprise Plan', price: '£100/mo' }
        ];
      }
    });
  }

  loadTenants() {
    this.tenantService.getTenants().subscribe({
      next: (data) => {
        debugger
        this.rawTenants = data;
      },
      error: () => {
        this.toast.error('Failed to load tenants from server');
      }
    });
  }

  // Pagination State
  currentPage = 1;
  pageSize = 5;
  mathMin = Math.min;

  // Sorting State
  sortField = 'companyName';
  sortAsc = true;

  getPlanName(planId: number | null): string {
    const plan = this.plans.find((p) => p.id === planId);
    return plan ? plan.name : 'No Plan';
  }

  get filteredAndSortedTenants() {
    let list = [...this.rawTenants];

    // 1. Filtering
    const q = this.searchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(
        (t) =>
          t.companyName.toLowerCase().includes(q) ||
          t.tenancyName.toLowerCase().includes(q) ||
          (t.email && t.email.toLowerCase().includes(q))
      );
    }

    // 2. Sorting
    list.sort((a, b) => {
      let valA: any = (a as any)[this.sortField] || '';
      let valB: any = (b as any)[this.sortField] || '';

      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();

      if (valA < valB) return this.sortAsc ? -1 : 1;
      if (valA > valB) return this.sortAsc ? 1 : -1;
      return 0;
    });

    return list;
  }

  get tenants() {
    const list = this.filteredAndSortedTenants;
    const start = (this.currentPage - 1) * this.pageSize;
    return list.slice(start, start + this.pageSize);
  }

  get totalPages() {
    return Math.ceil(this.filteredAndSortedTenants.length / this.pageSize) || 1;
  }

  setSort(field: string) {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = true;
    }
  }

  showDrawer = false;
  submitted = false;

  openCreateDrawer() {
    this.submitted = false;
    this.tenantForm.reset({ planId: 1 });
    this.showDrawer = true;
  }

  closeDrawer() {
    this.submitted = false;
    this.showDrawer = false;
    this.tenantForm.reset({ planId: 1 });
  }

  saveTenant() {
    this.submitted = true;
    if (this.tenantForm.invalid) {
      this.tenantForm.markAllAsTouched();
      return;
    }

    const payload = this.tenantForm.value;
    payload.planId = Number(payload.planId);

    this.tenantService.registerTenant(payload).subscribe({
      next: () => {
        this.toast.success('Tenant created successfully');
        this.loadTenants();
        this.closeDrawer();
      },
      error: (err) => {
        this.toast.error(err.error?.error || 'Failed to register tenant');
      }
    });
  }
}
