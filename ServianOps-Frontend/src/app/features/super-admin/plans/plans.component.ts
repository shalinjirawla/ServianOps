import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { LayoutService } from '../../../core/services/layout.service'

interface PlanDto {
  id: number;
  planName: string;
  maxUsers: number;
  maxProjects: number;
  maxStorageGB: number;
  price: number;
  billingCycle: string;
  isTrialAvailable: boolean;
  trialDays: number;
  isActive: boolean;
  creationTime: string;
}

@Component({
  selector: 'app-plans',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './plans.component.html',
  styleUrl: './plans.component.scss'
})
export class PlansComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly fb = inject(FormBuilder);
  private readonly layoutService = inject(LayoutService);

  // States
  readonly plansList = signal<PlanDto[]>([]);
  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);

  // Modal controllers
  readonly isModalOpen = signal(false);
  readonly editingPlanId = signal<number | null>(null);

  // Form Group
  planForm!: FormGroup;

  ngOnInit(): void {
    this.layoutService.setPageTitle('Plans Administration');
    this.initForm();
    this.fetchPlans();
  }

  initForm(): void {
    this.planForm = this.fb.group({
      planName: ['', [Validators.required]],
      maxUsers: [5, [Validators.required, Validators.min(1)]],
      maxProjects: [10, [Validators.required, Validators.min(1)]],
      maxStorageGB: [10, [Validators.required, Validators.min(1)]],
      price: [1000, [Validators.required, Validators.min(0)]],
      billingCycle: ['Monthly', [Validators.required]],
      isTrialAvailable: [true],
      trialDays: [14, [Validators.required, Validators.min(0)]],
      isActive: [true]
    });

    // Toggle trial days validation dynamically
    this.planForm.get('isTrialAvailable')?.valueChanges.subscribe((available) => {
      const trialDaysControl = this.planForm.get('trialDays');
      if (available) {
        trialDaysControl?.setValidators([Validators.required, Validators.min(0)]);
      } else {
        trialDaysControl?.clearValidators();
      }
      trialDaysControl?.updateValueAndValidity();
    });
  }

  fetchPlans(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.http.get<PlanDto[]>(`${environment.apiUrl}/api/plans`).subscribe({
      next: (res) => {
        this.plansList.set(res || []);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set('Failed to fetch subscription plans.');
        this.isLoading.set(false);
      }
    });
  }

  openCreateModal(): void {
    this.editingPlanId.set(null);
    this.planForm.reset({
      maxUsers: 5,
      maxProjects: 10,
      maxStorageGB: 10,
      price: 1000,
      billingCycle: 'Monthly',
      isTrialAvailable: true,
      trialDays: 14,
      isActive: true
    });
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(plan: PlanDto): void {
    this.editingPlanId.set(plan.id);
    this.planForm.patchValue({
      planName: plan.planName,
      maxUsers: plan.maxUsers,
      maxProjects: plan.maxProjects,
      maxStorageGB: plan.maxStorageGB,
      price: plan.price,
      billingCycle: plan.billingCycle,
      isTrialAvailable: plan.isTrialAvailable,
      trialDays: plan.trialDays,
      isActive: plan.isActive
    });
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.editingPlanId.set(null);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.planForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onSubmit(): void {
    if (this.planForm.invalid) {
      this.planForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    // DTO maps exactly: PascalCase fields in .NET backend, let's map keys to match backend's CreatePlanDto class properties
    const formVal = this.planForm.value;
    const payload = {
      PlanName: formVal.planName,
      MaxUsers: Number(formVal.maxUsers),
      MaxProjects: Number(formVal.maxProjects),
      MaxStorageGB: Number(formVal.maxStorageGB),
      Price: Number(formVal.price),
      BillingCycle: formVal.billingCycle,
      IsTrialAvailable: !!formVal.isTrialAvailable,
      TrialDays: Number(formVal.trialDays || 0),
      IsActive: !!formVal.isActive
    };

    const isEditing = this.editingPlanId() !== null;
    const request$ = isEditing
      ? this.http.put(`${environment.apiUrl}/api/plans/${this.editingPlanId()}`, payload)
      : this.http.post(`${environment.apiUrl}/api/plans`, payload);

    request$.subscribe({
      next: () => {
        this.successMessage.set(
          `Plan was successfully ${isEditing ? 'updated' : 'created'}!`
        );
        this.fetchPlans();
        this.isSaving.set(false);
        setTimeout(() => {
          this.closeModal();
        }, 1500);
      },
      error: (err) => {
        const msg = err.error?.error || 'Failed to save plan changes. Check inputs.';
        this.errorMessage.set(msg);
        this.isSaving.set(false);
      }
    });
  }

  deletePlan(planId: number): void {
    if (!confirm('Are you sure you want to delete this subscription plan?')) {
      return;
    }

    this.errorMessage.set(null);
    this.successMessage.set(null);

    this.http.delete(`${environment.apiUrl}/api/plans/${planId}`).subscribe({
      next: () => {
        this.successMessage.set('Subscription plan was deleted successfully.');
        this.fetchPlans();
      },
      error: (err) => {
        const msg = err.error?.error || 'Failed to delete plan. It might be assigned to active tenants.';
        this.errorMessage.set(msg);
      }
    });
  }
}
