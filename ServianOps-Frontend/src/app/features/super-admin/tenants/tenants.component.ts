import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { LayoutService } from '../../../core/services/layout.service';

interface TenantItem {
  id: number;
  companyName: string;
  tenancyName: string;
  planId: number | null;
  isActive: boolean;
}

interface PlanItem {
  id: number;
  planName: string;
  price: number;
}

@Component({
  selector: 'app-tenants',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './tenants.component.html',
  styleUrl: './tenants.component.scss'
})
export class TenantsComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly fb = inject(FormBuilder);
  private readonly layoutService = inject(LayoutService);

  // States
  readonly tenantsList = signal<TenantItem[]>([]);
  readonly plansList = signal<PlanItem[]>([]);
  readonly isLoading = signal(false);
  readonly isSaving = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);
  readonly isModalOpen = signal(false);

  // Form Group
  tenantForm!: FormGroup;

  ngOnInit(): void {
    this.layoutService.setPageTitle('Tenants Administration');
    this.initForm();
    this.fetchTenants();
    this.fetchPlans();
  }

  initForm(): void {
    this.tenantForm = this.fb.group({
      companyName: ['', [Validators.required]],
      tenancyName: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9-]+$/)]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required]],
      planId: [null],
      password: ['123qwe', [Validators.required, Validators.minLength(6)]] // Default initial admin password
    });
  }

  fetchTenants(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.http.get<any>(`${environment.apiUrl}/api/tenants?pageNumber=1&pageSize=100`).subscribe({
      next: (res) => {
        // Handle backend returns (usually a paged collection object with a items/data array, or raw array)
        const items = res.items || res.data || (Array.isArray(res) ? res : []);
        this.tenantsList.set(items);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set('Failed to fetch tenants. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  fetchPlans(): void {
    this.http.get<PlanItem[]>(`${environment.apiUrl}/api/plans`).subscribe({
      next: (res) => {
        this.plansList.set(res || []);
      },
      error: () => { } // Fail silently for plan select list
    });
  }

  openRegisterModal(): void {
    this.tenantForm.reset({
      password: '123qwe',
      planId: null
    });
    this.errorMessage.set(null);
    this.successMessage.set(null);
    this.isModalOpen.set(true);
  }

  closeRegisterModal(): void {
    this.isModalOpen.set(false);
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.tenantForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onSubmit(): void {
    if (this.tenantForm.invalid) {
      this.tenantForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const payload = this.tenantForm.value;
    // Map empty planId to null or number
    if (payload.planId) {
      payload.planId = Number(payload.planId);
    }

    this.http.post(`${environment.apiUrl}/api/auth/register-tenant`, payload).subscribe({
      next: () => {
        this.successMessage.set(`Tenant '${payload.companyName}' registered successfully!`);
        this.fetchTenants();
        this.isSaving.set(false);
        setTimeout(() => {
          this.closeRegisterModal();
        }, 1500);
      },
      error: (err) => {
        const msg = err.error?.error || 'Registration failed. Please check input parameters.';
        this.errorMessage.set(msg);
        this.isSaving.set(false);
      }
    });
  }
}
