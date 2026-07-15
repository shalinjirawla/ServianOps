import { Component, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { AuthService } from '../../core/services/auth.service';
import {
  PlansService,
  PlanListDto,
  RolesService,
  RoleListDto,
  TradesService,
  TradeListDto,
  CustomerTypesService,
  CustomerTypeListDto
} from '../../core/api/service-proxies';
import { ToastService } from '../../shared/toast/toast.service';
import { ConfirmationModalService } from '../../shared/confirmation-modal/confirmation-modal.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss',
})
export class SettingsComponent implements OnInit {
  isSuperAdmin = computed(() => this.auth.userRole() === 'SuperAdmin');
  submitted = false;
  activeTab: 'general' | 'plans' | 'roles' | 'trades' | 'customerTypes' = 'general';

  constructor(
    private auth: AuthService,
    private plansService: PlansService,
    private rolesService: RolesService,
    private tradesService: TradesService,
    private customerTypesService: CustomerTypesService,
    private toast: ToastService,
    private confirmSvc: ConfirmationModalService,
    private fb: FormBuilder
  ) { }

  // Existing settings fields
  companyName = 'ServianOps Services Ltd';
  vatNumber = 'GB 342 189 004';
  address = '17 Old Street, London EC1V 9HL';

  slaDefaults = [
    { p: 'P1', v: '4h' },
    { p: 'P2', v: '24h' },
    { p: 'P3', v: '3d' },
    { p: 'P4', v: '7d' },
  ];

  notifications = [
    { t: 'SLA breach warnings', d: 'Email + push when a job is 30 min from breaching', enabled: true },
    { t: 'Quote decisions', d: 'Notify when a client approves or declines a quote', enabled: true },
    { t: 'Engineer check-in alerts', d: 'If an engineer misses arrival window', enabled: true },
    { t: 'Weekly ops digest', d: 'Every Monday, 08:00', enabled: false },
  ];

  // Plan CRUD fields
  plans: PlanListDto[] = [];
  showPlanDrawer = false;
  isPlanEditMode = false;
  selectedPlanId: number | null = null;
  planForm!: FormGroup;

  // Role CRUD fields
  roles: RoleListDto[] = [];
  showRoleDrawer = false;
  isRoleEditMode = false;
  selectedRoleId: number | null = null;
  roleForm!: FormGroup;
  roleSubmitted = false;
  roleSearchQuery = '';
  roleCurrentPage = 1;
  rolePageSize = 5;
  roleSortField = 'name';
  roleSortAsc = true;

  // Trade CRUD fields
  trades: TradeListDto[] = [];
  showTradeDrawer = false;
  isTradeEditMode = false;
  selectedTradeId: number | null = null;
  tradeForm!: FormGroup;
  tradeSubmitted = false;
  tradeSearchQuery = '';
  tradeCurrentPage = 1;
  tradePageSize = 5;
  tradeSortField = 'name';
  tradeSortAsc = true;

  // Customer Type CRUD fields
  customerTypes: CustomerTypeListDto[] = [];
  showCustomerTypeDrawer = false;
  isCustomerTypeEditMode = false;
  selectedCustomerTypeId: number | null = null;
  customerTypeForm!: FormGroup;
  customerTypeSubmitted = false;
  customerTypeSearchQuery = '';
  customerTypeCurrentPage = 1;
  customerTypePageSize = 5;
  customerTypeSortField = 'name';
  customerTypeSortAsc = true;

  mathMin = Math.min;

  ngOnInit() {
    this.initForms();
    if (this.isSuperAdmin()) {
      this.activeTab = 'plans';
      this.loadPlans();
    } else {
      this.activeTab = 'general';
      this.loadRoles();
      this.loadTrades();
      this.loadCustomerTypes();
    }
  }

  initForms() {
    this.planForm = this.fb.group({
      planName: ['', [Validators.required]],
      maxUsers: [10, [Validators.required, Validators.min(1)]],
      maxProjects: [50, [Validators.required, Validators.min(1)]],
      maxStorageGB: [10, [Validators.required, Validators.min(1)]],
      price: [0, [Validators.required, Validators.min(0)]],
      billingCycle: ['Monthly', [Validators.required]],
      isTrialAvailable: [true],
      trialDays: [14, [Validators.min(0)]],
      isActive: [true]
    });

    this.roleForm = this.fb.group({
      name: ['', [Validators.required]],
      description: ['']
    });

    this.tradeForm = this.fb.group({
      name: ['', [Validators.required]]
    });

    this.customerTypeForm = this.fb.group({
      name: ['', [Validators.required]]
    });
  }

  setTab(tab: 'general' | 'plans' | 'roles' | 'trades' | 'customerTypes') {
    this.activeTab = tab;
  }

  loadPlans() {
    this.plansService.getAllPlans(undefined, undefined, undefined, undefined, 1, 200).subscribe({
      next: (res) => {
        this.plans = res.data?.items || [];
      },
      error: () => {
        this.toast.error('Failed to load billing plans');
      }
    });
  }

  toggle(i: number) {
    this.notifications[i].enabled = !this.notifications[i].enabled;
  }

  // --- Plan CRUD Actions ---
  openCreatePlanDrawer() {
    this.isPlanEditMode = false;
    this.selectedPlanId = null;
    this.submitted = false;
    this.planForm.reset({
      maxUsers: 10,
      maxProjects: 50,
      maxStorageGB: 10,
      price: 0,
      billingCycle: 'Monthly',
      isTrialAvailable: true,
      trialDays: 14,
      isActive: true
    });
    this.showPlanDrawer = true;
  }

  openEditPlanDrawer(p: PlanListDto) {
    this.isPlanEditMode = true;
    this.selectedPlanId = p.id!;
    this.submitted = false;
    this.planForm.patchValue({
      planName: p.planName,
      maxUsers: p.maxUsers,
      maxProjects: p.maxProjects,
      maxStorageGB: p.maxStorageGB,
      price: p.price ? p.price / 100 : 0,
      billingCycle: p.billingCycle,
      isTrialAvailable: p.isTrialAvailable,
      trialDays: p.trialDays,
      isActive: p.isActive
    });
    this.showPlanDrawer = true;
  }

  closePlanDrawer() {
    this.submitted = false;
    this.showPlanDrawer = false;
    this.planForm.reset();
  }

  savePlan() {
    this.submitted = true;
    if (this.planForm.invalid) {
      this.planForm.markAllAsTouched();
      return;
    }

    const val = this.planForm.value;
    const payload = {
      planName: val.planName,
      maxUsers: Number(val.maxUsers),
      maxProjects: Number(val.maxProjects),
      maxStorageGB: Number(val.maxStorageGB),
      price: Math.round(Number(val.price) * 100),
      billingCycle: val.billingCycle,
      isTrialAvailable: val.isTrialAvailable,
      trialDays: Number(val.trialDays),
      isActive: val.isActive
    };

    if (this.isPlanEditMode && this.selectedPlanId) {
      this.plansService.updatePlan(this.selectedPlanId, payload).subscribe({
        next: () => {
          this.toast.success('Billing plan updated successfully');
          this.loadPlans();
          this.closePlanDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to update billing plan');
        }
      });
    } else {
      this.plansService.createPlan(payload).subscribe({
        next: () => {
          this.toast.success('Billing plan created successfully');
          this.loadPlans();
          this.closePlanDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to create billing plan');
        }
      });
    }
  }

  deletePlan(id: number) {
    this.confirmSvc.confirm(
      'Delete Billing Plan',
      'Are you sure you want to delete this billing plan? This action cannot be undone.'
    ).subscribe((confirmed) => {
      if (confirmed) {
        this.plansService.deletePlan(id).subscribe({
          next: () => {
            this.toast.success('Billing plan deleted successfully');
            this.loadPlans();
          },
          error: (err) => {
            this.toast.error(err.error?.error || 'Failed to delete billing plan');
          }
        });
      }
    });
  }

  // --- Role CRUD Handlers ---
  loadRoles() {
    // this.rolesService.getAllRoles(undefined, 1, 200).subscribe({
    //   next: (res) => {
    //     this.roles = res.data?.items || [];
    //   },
    //   error: () => {
    //     this.toast.error('Failed to load user roles');
    //   }
    // });
  }

  get filteredAndSortedRoles() {
    let list = [...this.roles];
    const q = this.roleSearchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(r =>
        (r.name || '').toLowerCase().includes(q) ||
        (r.description || '').toLowerCase().includes(q)
      );
    }
    list.sort((a, b) => {
      let valA: any = (a as any)[this.roleSortField] || '';
      let valB: any = (b as any)[this.roleSortField] || '';
      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();
      if (valA < valB) return this.roleSortAsc ? -1 : 1;
      if (valA > valB) return this.roleSortAsc ? 1 : -1;
      return 0;
    });
    return list;
  }

  get paginatedRoles() {
    const list = this.filteredAndSortedRoles;
    const start = (this.roleCurrentPage - 1) * this.rolePageSize;
    return list.slice(start, start + this.rolePageSize);
  }

  get roleTotalPages() {
    return Math.ceil(this.filteredAndSortedRoles.length / this.rolePageSize) || 1;
  }

  setRoleSort(field: string) {
    if (this.roleSortField === field) {
      this.roleSortAsc = !this.roleSortAsc;
    } else {
      this.roleSortField = field;
      this.roleSortAsc = true;
    }
  }

  openCreateRoleDrawer() {
    this.isRoleEditMode = false;
    this.selectedRoleId = null;
    this.roleSubmitted = false;
    this.roleForm.reset();
    this.showRoleDrawer = true;
  }

  openEditRoleDrawer(r: RoleListDto) {
    this.isRoleEditMode = true;
    this.selectedRoleId = r.id!;
    this.roleSubmitted = false;
    this.roleForm.patchValue({
      name: r.name,
      description: r.description
    });
    this.showRoleDrawer = true;
  }

  closeRoleDrawer() {
    this.roleSubmitted = false;
    this.showRoleDrawer = false;
    this.roleForm.reset();
  }

  saveRole() {
    this.roleSubmitted = true;
    if (this.roleForm.invalid) {
      this.roleForm.markAllAsTouched();
      return;
    }

    const payload = this.roleForm.value;
    if (this.isRoleEditMode && this.selectedRoleId) {
      this.rolesService.updateRole(this.selectedRoleId, payload).subscribe({
        next: () => {
          this.toast.success('User role updated successfully');
          this.loadRoles();
          this.closeRoleDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to update role');
        }
      });
    } else {
      this.rolesService.createRole(payload).subscribe({
        next: () => {
          this.toast.success('User role created successfully');
          this.loadRoles();
          this.closeRoleDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to create role');
        }
      });
    }
  }

  deleteRole(id: number) {
    this.confirmSvc.confirm(
      'Delete User Role',
      'Are you sure you want to delete this user role? Users assigned to this role might lose system permissions. This action cannot be undone.'
    ).subscribe((confirmed) => {
      if (confirmed) {
        this.rolesService.deleteRole(id).subscribe({
          next: () => {
            this.toast.success('User role deleted successfully');
            this.loadRoles();
          },
          error: (err) => {
            this.toast.error(err.error?.error || 'Failed to delete role');
          }
        });
      }
    });
  }

  // --- Trade CRUD Handlers ---
  loadTrades() {
    // this.tradesService.getAllTrades(undefined, undefined, "1", "200").subscribe({
    //   next: (res) => {
    //     this.trades = res.data?.items || [];
    //   },
    //   error: () => {
    //     this.toast.error('Failed to load trade categories');
    //   }
    // });
  }

  get filteredAndSortedTrades() {
    let list = [...this.trades];
    const q = this.tradeSearchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(t => (t.name || '').toLowerCase().includes(q));
    }
    list.sort((a, b) => {
      let valA: any = (a as any)[this.tradeSortField] || '';
      let valB: any = (b as any)[this.tradeSortField] || '';
      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();
      if (valA < valB) return this.tradeSortAsc ? -1 : 1;
      if (valA > valB) return this.tradeSortAsc ? 1 : -1;
      return 0;
    });
    return list;
  }

  get paginatedTrades() {
    const list = this.filteredAndSortedTrades;
    const start = (this.tradeCurrentPage - 1) * this.tradePageSize;
    return list.slice(start, start + this.tradePageSize);
  }

  get tradeTotalPages() {
    return Math.ceil(this.filteredAndSortedTrades.length / this.tradePageSize) || 1;
  }

  setTradeSort(field: string) {
    if (this.tradeSortField === field) {
      this.tradeSortAsc = !this.tradeSortAsc;
    } else {
      this.tradeSortField = field;
      this.tradeSortAsc = true;
    }
  }

  openCreateTradeDrawer() {
    this.isTradeEditMode = false;
    this.selectedTradeId = null;
    this.tradeSubmitted = false;
    this.tradeForm.reset();
    this.showTradeDrawer = true;
  }

  openEditTradeDrawer(t: TradeListDto) {
    this.isTradeEditMode = true;
    this.selectedTradeId = t.id!;
    this.tradeSubmitted = false;
    this.tradeForm.patchValue({
      name: t.name
    });
    this.showTradeDrawer = true;
  }

  closeTradeDrawer() {
    this.tradeSubmitted = false;
    this.showTradeDrawer = false;
    this.tradeForm.reset();
  }

  saveTrade() {
    this.tradeSubmitted = true;
    if (this.tradeForm.invalid) {
      this.tradeForm.markAllAsTouched();
      return;
    }

    const payload = this.tradeForm.value;
    if (this.isTradeEditMode && this.selectedTradeId) {
      this.tradesService.updateTrade(this.selectedTradeId, payload).subscribe({
        next: () => {
          this.toast.success('Trade category updated successfully');
          this.loadTrades();
          this.closeTradeDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to update trade');
        }
      });
    } else {
      this.tradesService.createTrade(payload).subscribe({
        next: () => {
          this.toast.success('Trade category created successfully');
          this.loadTrades();
          this.closeTradeDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to create trade');
        }
      });
    }
  }

  deleteTrade(id: number) {
    this.confirmSvc.confirm(
      'Delete Trade Category',
      'Are you sure you want to delete this trade category? Associated job allocations and engineering settings will be affected. This action cannot be undone.'
    ).subscribe((confirmed) => {
      if (confirmed) {
        this.tradesService.deleteTrade(id).subscribe({
          next: () => {
            this.toast.success('Trade category deleted successfully');
            this.loadTrades();
          },
          error: (err) => {
            this.toast.error(err.error?.error || 'Failed to delete trade');
          }
        });
      }
    });
  }

  // --- Customer Type CRUD Handlers ---
  loadCustomerTypes() {
    // this.customerTypesService.getAllCustomerTypes(undefined, 1, 200).subscribe({
    //   next: (res) => {
    //     this.customerTypes = res.data?.items || [];
    //   },
    //   error: () => {
    //     this.toast.error('Failed to load customer profile categories');
    //   }
    // });
  }

  get filteredAndSortedCustomerTypes() {
    let list = [...this.customerTypes];
    const q = this.customerTypeSearchQuery.toLowerCase().trim();
    if (q) {
      list = list.filter(ct => (ct.name || '').toLowerCase().includes(q));
    }
    list.sort((a, b) => {
      let valA: any = (a as any)[this.customerTypeSortField] || '';
      let valB: any = (b as any)[this.customerTypeSortField] || '';
      if (typeof valA === 'string') valA = valA.toLowerCase();
      if (typeof valB === 'string') valB = valB.toLowerCase();
      if (valA < valB) return this.customerTypeSortAsc ? -1 : 1;
      if (valA > valB) return this.customerTypeSortAsc ? 1 : -1;
      return 0;
    });
    return list;
  }

  get paginatedCustomerTypes() {
    const list = this.filteredAndSortedCustomerTypes;
    const start = (this.customerTypeCurrentPage - 1) * this.customerTypePageSize;
    return list.slice(start, start + this.customerTypePageSize);
  }

  get customerTypeTotalPages() {
    return Math.ceil(this.filteredAndSortedCustomerTypes.length / this.customerTypePageSize) || 1;
  }

  setCustomerTypeSort(field: string) {
    if (this.customerTypeSortField === field) {
      this.customerTypeSortAsc = !this.customerTypeSortAsc;
    } else {
      this.customerTypeSortField = field;
      this.customerTypeSortAsc = true;
    }
  }

  openCreateCustomerTypeDrawer() {
    this.isCustomerTypeEditMode = false;
    this.selectedCustomerTypeId = null;
    this.customerTypeSubmitted = false;
    this.customerTypeForm.reset();
    this.showCustomerTypeDrawer = true;
  }

  openEditCustomerTypeDrawer(ct: CustomerTypeListDto) {
    this.isCustomerTypeEditMode = true;
    this.selectedCustomerTypeId = ct.id!;
    this.customerTypeSubmitted = false;
    this.customerTypeForm.patchValue({
      name: ct.name
    });
    this.showCustomerTypeDrawer = true;
  }

  closeCustomerTypeDrawer() {
    this.customerTypeSubmitted = false;
    this.showCustomerTypeDrawer = false;
    this.customerTypeForm.reset();
  }

  saveCustomerType() {
    this.customerTypeSubmitted = true;
    if (this.customerTypeForm.invalid) {
      this.customerTypeForm.markAllAsTouched();
      return;
    }

    const payload = this.customerTypeForm.value;
    if (this.isCustomerTypeEditMode && this.selectedCustomerTypeId) {
      this.customerTypesService.updateCustomerType(this.selectedCustomerTypeId, payload).subscribe({
        next: () => {
          this.toast.success('Customer profile category updated successfully');
          this.loadCustomerTypes();
          this.closeCustomerTypeDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to update customer type');
        }
      });
    } else {
      this.customerTypesService.createCustomerType(payload).subscribe({
        next: () => {
          this.toast.success('Customer profile category created successfully');
          this.loadCustomerTypes();
          this.closeCustomerTypeDrawer();
        },
        error: (err) => {
          this.toast.error(err.error?.error || 'Failed to create customer type');
        }
      });
    }
  }

  deleteCustomerType(id: number) {
    this.confirmSvc.confirm(
      'Delete Customer Category',
      'Are you sure you want to delete this customer category? Clients assigned to this category will revert to general settings. This action cannot be undone.'
    ).subscribe((confirmed) => {
      if (confirmed) {
        this.customerTypesService.deleteCustomerType(id).subscribe({
          next: () => {
            this.toast.success('Customer category deleted successfully');
            this.loadCustomerTypes();
          },
          error: (err) => {
            this.toast.error(err.error?.error || 'Failed to delete customer category');
          }
        });
      }
    });
  }
}
