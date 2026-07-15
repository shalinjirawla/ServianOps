import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { ToastService } from '../../shared/toast/toast.service';
import { ConfirmationModalService } from '../../shared/confirmation-modal/confirmation-modal.service';
import {
  TenantsService,
  AuthService,
  PlansService,
  CreateTenantDto,
  TenantListDto,
  PlanListDto,
  UpdateTenantDto,
  TenantDetailDto,
} from '../../core/api/service-proxies';

@Component({
  selector: 'app-tenants',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './tenants.component.html',
  styleUrl: './tenants.component.scss',
})
export class TenantsComponent implements OnInit, OnDestroy {
  // ---- Data ----
  tenants: TenantListDto[] = [];
  plans: PlanListDto[] = [];
  totalCount = 0;

  // ---- UI state ----
  loading = false;
  deletingId: number | null = null;
  loadError = false;
  loadingDetails = false;
  formErrors: string[] = [];
  loadedTenantDetail: TenantDetailDto | null = null;

  // ---- Search ----
  searchQuery = '';
  private searchInput$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  // ---- Pagination (server-driven) ----
  currentPage = 1;
  pageSize = 5;
  mathMin = Math.min;

  // ---- Sorting (server-driven) ----
  sortField = 'companyName';
  sortAsc = true;

  // ---- Form / Drawer ----
  tenantForm!: FormGroup;
  timeZones: { value: string; label: string }[] = [];
  currencies: { value: string; label: string }[] = [];
  isEditMode = false;
  selectedTenantId: number | null = null;
  showDrawer = false;
  submitted = false;
  saving = false;

  logoPreviewUrl: string | null = null;
  logoFileError: string | null = null;
  private readonly maxLogoSizeBytes = 2 * 1024 * 1024; // 2MB
  private readonly allowedLogoTypes = ['image/png', 'image/jpeg', 'image/svg+xml', 'image/webp'];
  private pendingLogoFile: File | null = null;

  constructor(
    private tenantService: TenantsService,
    private planService: PlansService,
    private authService: AuthService,
    private toast: ToastService,
    private confirmSvc: ConfirmationModalService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.loadTimeZones();
    this.loadCurrencies();
    this.initForm();
    this.loadPlans();
    this.loadTenants();

    // Debounce search so we don't hammer the API on every keystroke
    this.searchInput$
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(() => {
        this.currentPage = 1;
        this.loadTenants();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ============================================================
  // Form
  // ============================================================
  initForm(): void {
    const defaultTz = Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC';

    this.tenantForm = this.fb.group({
      companyName: ['', [Validators.required]],
      tenancyName: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9-]+$'), Validators.minLength(2)]],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern('^(\\+?[0-9\\s\\-\\(\\)]{7,20})?$')]],
      logoUrl: ['', [Validators.required]],
      timeZone: [defaultTz, [Validators.required]],
      currency: ['GBP', [Validators.required]],
      planId: [1, [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  // ============================================================
  // Data loading — server-side search / sort / paging
  // ============================================================
  loadPlans(): void {
    this.planService.getAllPlans(undefined, true, undefined, undefined, 1, 100, 'planName', 'asc').subscribe({
      next: (res) => {
        this.plans = res?.data?.items ?? [];
      },
      error: () => {
        this.plans = [];
        this.toast.error('Failed to load billing plans');
      },
    });
  }

  loadTenants(): void {
    this.loading = true;
    this.loadError = false;

    const search = this.searchQuery.trim() || undefined;
    const sortDirection = this.sortAsc ? 'asc' : 'desc';

    this.tenantService
      .getAllTenants(
        search,
        undefined, // isActive
        undefined, // dateFrom
        undefined, // dateTo
        this.currentPage,
        this.pageSize,
        this.sortField,
        sortDirection
      )
      .subscribe({
        next: (res) => {
          this.tenants = res?.data?.items ?? [];
          this.totalCount = res?.data?.totalCount ?? 0;
          this.loading = false;

          // If we deleted the last item on a page, step back a page
          if (this.tenants.length === 0 && this.currentPage > 1) {
            this.currentPage -= 1;
            this.loadTenants();
          }
        },
        error: () => {
          this.loading = false;
          this.loadError = true;
          this.tenants = [];
          this.totalCount = 0;
          this.toast.error('Failed to load tenants from server');
        },
      });
  }

  onSearchChange(value: string): void {
    this.searchQuery = value;
    this.searchInput$.next(value);
  }

  setSort(field: string): void {
    if (this.sortField === field) {
      this.sortAsc = !this.sortAsc;
    } else {
      this.sortField = field;
      this.sortAsc = true;
    }
    this.currentPage = 1;
    this.loadTenants();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadTenants();
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize) || 1;
  }

  get rangeStart(): number {
    return this.totalCount === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  get rangeEnd(): number {
    return this.mathMin(this.currentPage * this.pageSize, this.totalCount);
  }

  // ============================================================
  // Helpers
  // ============================================================
  getPlanName(planId: number | null | undefined): string {
    if (planId === null || planId === undefined) return 'No Plan';
    const plan = this.plans.find((p) => p.id === planId);
    return plan?.planName || 'No Plan';
  }

  getAdminUser(t: TenantListDto) {
    return t.adminUser;
  }

  // ============================================================
  // Drawer: create / edit
  // ============================================================
  openCreateDrawer(): void {
    this.isEditMode = false;
    this.selectedTenantId = null;
    this.submitted = false;
    this.saving = false;
    this.logoPreviewUrl = null;
    this.logoFileError = null;
    this.pendingLogoFile = null;
    this.formErrors = [];
    this.loadedTenantDetail = null;

    const defaultTz = Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC';
    this.tenantForm.reset({ planId: this.plans[0]?.id ?? 1, timeZone: defaultTz, currency: 'GBP' });

    this.setAdminFieldValidators(true);
    this.showDrawer = true;
  }

  openEditDrawer(t: TenantListDto): void {
    this.isEditMode = true;
    this.selectedTenantId = t.id ?? null;
    this.submitted = false;
    this.saving = false;
    this.logoPreviewUrl = null;
    this.logoFileError = null;
    this.pendingLogoFile = null;
    this.formErrors = [];
    this.loadedTenantDetail = null;

    this.tenantForm.reset();
    this.setAdminFieldValidators(false);
    this.showDrawer = true;
    this.loadingDetails = true;

    this.tenantService.getTenantById(t.id!).subscribe({
      next: (res) => {
        this.loadingDetails = false;
        if (res.success && res.data) {
          const detail = res.data;
          this.loadedTenantDetail = detail;
          this.logoPreviewUrl = detail.logoUrl || null;
          this.tenantForm.reset({
            companyName: detail.companyName,
            tenancyName: detail.tenancyName,
            logoUrl: detail.logoUrl || '',
            timeZone: detail.timeZone || 'UTC',
            currency: detail.currency || 'GBP',
            planId: detail.plan?.id ?? this.plans[0]?.id ?? 1,
            firstName: '',
            lastName: '',
            email: '',
            phone: '',
            password: '',
          });
        } else {
          this.toast.error(res.message || 'Failed to load tenant details');
          this.closeDrawer();
        }
      },
      error: (err) => {
        this.loadingDetails = false;
        this.toast.error(err?.error?.message || err?.error?.error || 'Failed to load tenant details from server');
        this.closeDrawer();
      }
    });
  }

  private setAdminFieldValidators(required: boolean): void {
    const fields = ['firstName', 'lastName', 'password'] as const;
    fields.forEach((f) => {
      const ctrl = this.tenantForm.get(f);
      if (!ctrl) return;
      ctrl.setValidators(required ? [Validators.required] : []);
      ctrl.updateValueAndValidity();
    });

    const email = this.tenantForm.get('email');
    if (email) {
      email.setValidators(required ? [Validators.required, Validators.email] : []);
      email.updateValueAndValidity();
    }

    if (required) {
      const pwd = this.tenantForm.get('password');
      pwd?.setValidators([Validators.required, Validators.minLength(6)]);
      pwd?.updateValueAndValidity();
    }
  }

  closeDrawer(): void {
    this.submitted = false;
    this.showDrawer = false;
    this.saving = false;
    this.logoPreviewUrl = null;
    this.logoFileError = null;
    this.pendingLogoFile = null;
    this.formErrors = [];
    this.loadingDetails = false;
    this.loadedTenantDetail = null;
    const defaultTz = Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC';
    this.tenantForm.reset({ planId: this.plans[0]?.id ?? 1, timeZone: defaultTz, currency: 'GBP' });
  }

  saveTenant(): void {
    this.submitted = true;
    this.formErrors = [];
    if (this.tenantForm.invalid || this.saving) {
      this.tenantForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const val = this.tenantForm.getRawValue();

    if (this.isEditMode && this.selectedTenantId) {
      const payload: UpdateTenantDto = {
        companyName: val.companyName,
        tenancyName: val.tenancyName,
        planId: Number(val.planId),
        logoUrl: val.logoUrl,
        timeZone: val.timeZone,
        currency: val.currency,
        isTrial: this.loadedTenantDetail?.isTrial ?? false,
        isExpired: this.loadedTenantDetail?.isExpired ?? false,
        subscriptionStartDate: this.loadedTenantDetail?.subscriptionStartDate,
        subscriptionEndDate: this.loadedTenantDetail?.subscriptionEndDate,
      };

      this.tenantService.updateTenant(this.selectedTenantId, payload).subscribe({
        next: (res) => {
          this.saving = false;
          if (res.success) {
            this.toast.success('Tenant updated successfully');
            this.loadTenants();
            this.closeDrawer();
          } else {
            this.formErrors = res.errors && res.errors.length > 0 ? res.errors : [res.message || 'Failed to update tenant'];
            this.toast.error(res.message || 'Failed to update tenant');
          }
        },
        error: (err) => {
          this.saving = false;
          const apiErrors = err?.error?.errors || [];
          const apiMsg = err?.error?.message || err?.error?.error || 'Failed to update tenant';
          this.formErrors = apiErrors.length > 0 ? apiErrors : [apiMsg];
          this.toast.error(apiMsg);
        },
      });
    } else {
      const payload: CreateTenantDto = {
        companyName: val.companyName,
        tenancyName: val.tenancyName,
        firstName: val.firstName,
        lastName: val.lastName,
        email: val.email,
        phone: val.phone || undefined,
        planId: Number(val.planId),
        password: val.password,
      };

      this.tenantService.createTenant(payload).subscribe({
        next: (res) => {
          this.saving = false;
          if (res.success) {
            this.toast.success('Tenant created successfully');
            this.currentPage = 1;
            this.loadTenants();
            this.closeDrawer();
          } else {
            this.formErrors = res.errors && res.errors.length > 0 ? res.errors : [res.message || 'Failed to create tenant'];
            this.toast.error(res.message || 'Failed to create tenant');
          }
        },
        error: (err) => {
          this.saving = false;
          const apiErrors = err?.error?.errors || [];
          const apiMsg = err?.error?.message || err?.error?.error || 'Failed to create tenant';
          this.formErrors = apiErrors.length > 0 ? apiErrors : [apiMsg];
          this.toast.error(apiMsg);
        },
      });
    }
  }

  deleteTenant(id: number): void {
    this.confirmSvc
      .confirm(
        'Delete Tenant Account',
        'Are you sure you want to delete this tenant and all associated databases and users? This action is permanent and cannot be undone.'
      )
      .subscribe((confirmed) => {
        if (!confirmed) return;

        this.deletingId = id;
        this.tenantService.deleteTenant(id).subscribe({
          next: () => {
            this.deletingId = null;
            this.toast.success('Tenant deleted successfully');
            this.loadTenants();
          },
          error: (err) => {
            this.deletingId = null;
            this.toast.error(err?.error?.message || err?.error?.error || 'Failed to delete tenant');
          },
        });
      });
  }

  // ============================================================
  // Logo upload (local preview; swap in real upload endpoint when available)
  // ============================================================
  onLogoSelected(event: Event): void {
    this.logoFileError = null;
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      this.clearLogoValue(input);
      return;
    }

    if (!this.allowedLogoTypes.includes(file.type)) {
      this.logoFileError = 'Unsupported file type. Use PNG, JPG, SVG, or WebP.';
      this.clearLogoValue(input);
      return;
    }

    if (file.size > this.maxLogoSizeBytes) {
      this.logoFileError = 'File is too large. Max size is 2MB.';
      this.clearLogoValue(input);
      return;
    }

    this.pendingLogoFile = file;
    const reader = new FileReader();
    reader.onload = () => {
      const dataUrl = reader.result as string;
      this.logoPreviewUrl = dataUrl;
      // TODO: replace with a real upload endpoint once available, e.g.:
      // this.tenantService.uploadLogo(file).subscribe(res => {
      //   this.logoPreviewUrl = res.data.url;
      //   this.tenantForm.patchValue({ logoUrl: res.data.url });
      //   this.tenantForm.get('logoUrl')?.markAsDirty();
      // });
      this.tenantForm.patchValue({ logoUrl: dataUrl });
      this.tenantForm.get('logoUrl')?.markAsDirty();
    };
    reader.onerror = () => {
      this.logoFileError = 'Failed to read the selected file.';
      this.clearLogoValue(input);
    };
    reader.readAsDataURL(file);
  }

  removeLogo(fileInput: HTMLInputElement): void {
    this.clearLogoValue(fileInput);
  }

  private clearLogoValue(input: HTMLInputElement): void {
    this.logoPreviewUrl = null;
    this.pendingLogoFile = null;
    this.tenantForm.patchValue({ logoUrl: '' });
    input.value = '';
  }

  // ============================================================
  // Time zones / currencies
  // ============================================================
  private loadTimeZones(): void {
    let zones: string[];
    try {
      zones = (Intl as any).supportedValuesOf ? (Intl as any).supportedValuesOf('timeZone') : this.fallbackTimeZones();
    } catch {
      zones = this.fallbackTimeZones();
    }

    this.timeZones = zones
      .map((tz) => ({ value: tz, label: this.formatTimeZoneLabel(tz) }))
      .sort((a, b) => a.label.localeCompare(b.label));
  }

  private fallbackTimeZones(): string[] {
    return [
      'UTC',
      'Europe/London',
      'Europe/Berlin',
      'Europe/Paris',
      'America/New_York',
      'America/Chicago',
      'America/Los_Angeles',
      'Asia/Kolkata',
      'Asia/Dubai',
      'Asia/Singapore',
      'Asia/Tokyo',
      'Australia/Sydney',
    ];
  }

  private formatTimeZoneLabel(tz: string): string {
    try {
      const now = new Date();
      const offset =
        new Intl.DateTimeFormat('en-GB', { timeZone: tz, timeZoneName: 'shortOffset' })
          .formatToParts(now)
          .find((p) => p.type === 'timeZoneName')?.value ?? '';
      return `${tz.replace(/_/g, ' ')} (${offset})`;
    } catch {
      return tz.replace(/_/g, ' ');
    }
  }

  private loadCurrencies(): void {
    const codes = ['GBP', 'USD', 'EUR', 'INR', 'JPY', 'AUD', 'CAD', 'CHF', 'CNY', 'SGD', 'AED', 'ZAR', 'NZD', 'SEK', 'NOK'];

    let displayNames: Intl.DisplayNames | null = null;
    try {
      displayNames = new Intl.DisplayNames(['en'], { type: 'currency' });
    } catch {
      displayNames = null;
    }

    this.currencies = codes
      .map((code) => ({ value: code, label: displayNames ? `${code} — ${displayNames.of(code)}` : code }))
      .sort((a, b) => a.value.localeCompare(b.value));
  }
}