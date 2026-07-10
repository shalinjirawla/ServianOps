import { Component, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { AuthService } from '../../core/services/auth.service';
import { PlanService, Plan } from '../../core/services/plan.service';
import { ToastService } from '../../shared/toast/toast.service';
import { ConfirmationModalService } from '../../shared/confirmation-modal/confirmation-modal.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TopbarComponent, IconComponent],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss',
})
export class SettingsComponent implements OnInit {
  isSuperAdmin = computed(() => this.auth.userRole() === 'SuperAdmin');
  submitted = false;

  constructor(
    private auth: AuthService,
    private planService: PlanService,
    private toast: ToastService,
    private confirmSvc: ConfirmationModalService,
    private fb: FormBuilder
  ) {}

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
  plans: Plan[] = [];
  showPlanDrawer = false;
  isPlanEditMode = false;
  selectedPlanId: number | null = null;

  // Plan Form Fields
  planForm!: FormGroup;

  ngOnInit() {
    this.initForm();
    if (this.isSuperAdmin()) {
      this.loadPlans();
    }
  }

  initForm() {
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
  }

  loadPlans() {
    this.planService.getPlans().subscribe({
      next: (data) => {
        this.plans = data;
      },
      error: () => {
        this.toast.error('Failed to load billing plans');
      }
    });
  }

  toggle(i: number) {
    this.notifications[i].enabled = !this.notifications[i].enabled;
  }

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

  openEditPlanDrawer(p: Plan) {
    this.isPlanEditMode = true;
    this.selectedPlanId = p.id;
    this.submitted = false;
    this.planForm.patchValue({
      planName: p.planName,
      maxUsers: p.maxUsers,
      maxProjects: p.maxProjects,
      maxStorageGB: p.maxStorageGB,
      price: p.price / 100,
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
    const payload: Partial<Plan> = {
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
      this.planService.updatePlan(this.selectedPlanId, payload).subscribe({
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
      this.planService.createPlan(payload).subscribe({
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
        this.planService.deletePlan(id).subscribe({
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
}
