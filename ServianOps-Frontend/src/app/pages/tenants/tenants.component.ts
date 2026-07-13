import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../layout/topbar/topbar.component';
import { IconComponent } from '../../shared/icon/icon.component';
import { ToastService } from '../../shared/toast/toast.service';
import { TenantService, TenantDto, CreateTenantDto } from '../../core/services/tenant.service';
import { PlanService } from '../../core/services/plan.service';
import { ConfirmationModalService } from '../../shared/confirmation-modal/confirmation-modal.service';
import { UserSummaryDto } from '../../core/services/user.service';

@Component({
  selector: 'app-tenants',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, TopbarComponent, IconComponent],
  templateUrl: './tenants.component.html',
  styleUrl: './tenants.component.scss',
})
export class TenantsComponent implements OnInit {
  searchQuery = '';
  rawTenants: TenantDto[] = [];
  plans: any[] = [];
  tenantForm!: FormGroup;

  timeZones: { value: string; label: string }[] = [];
  currencies: { value: string; label: string }[] = [];

  constructor(
    private tenantService: TenantService,
    private planService: PlanService,
    private toast: ToastService,
    private confirmSvc: ConfirmationModalService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.loadTimeZones();
    this.loadCurrencies();
    this.loadPlans();
    this.loadTenants();
    this.initForm();
  }

  initForm() {
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
      error: () => {
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

  // Per-row admin user — the previous single shared `adminUser` property
  // caused every row to show the first tenant's admin.
  getAdminUser(t: TenantDto): UserSummaryDto | undefined {
    return t.users?.[0];
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
          (t.users[0]?.email && t.users[0].email.toLowerCase().includes(q))
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
    this.logoPreviewUrl = null;
    this.logoFileError = null;
    const defaultTz = Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC';
    this.tenantForm.reset({ planId: 1, timeZone: defaultTz, currency: 'GBP' });
    this.showDrawer = true;
  }

  closeDrawer() {
    this.submitted = false;
    this.showDrawer = false;
    this.logoPreviewUrl = null;
    this.logoFileError = null;
    const defaultTz = Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC';
    this.tenantForm.reset({ planId: 1, timeZone: defaultTz, currency: 'GBP' });
  }

  saveTenant() {
    this.submitted = true;
    if (this.tenantForm.invalid) {
      this.tenantForm.markAllAsTouched();
      return;
    }

    const payload: CreateTenantDto = this.tenantForm.value;
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
  logoPreviewUrl: string | null = null;
  logoFileError: string | null = null;
  private readonly maxLogoSizeBytes = 2 * 1024 * 1024; // 2MB
  private readonly allowedLogoTypes = ['image/png', 'image/jpeg', 'image/svg+xml', 'image/webp'];
  onLogoSelected(event: Event) {
    this.logoFileError = null;
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) {
      this.logoPreviewUrl = null;
      this.tenantForm.patchValue({ logoUrl: '' });
      return;
    }

    if (!this.allowedLogoTypes.includes(file.type)) {
      this.logoFileError = 'Unsupported file type. Use PNG, JPG, SVG, or WebP.';
      input.value = '';
      this.logoPreviewUrl = null;
      this.tenantForm.patchValue({ logoUrl: '' });
      return;
    }

    if (file.size > this.maxLogoSizeBytes) {
      this.logoFileError = 'File is too large. Max size is 2MB.';
      input.value = '';
      this.logoPreviewUrl = null;
      this.tenantForm.patchValue({ logoUrl: '' });
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      const dataUrl = reader.result as string;
      this.logoPreviewUrl = dataUrl;
      // NOTE: if you add a real upload endpoint later, upload `file` here
      // instead and patchValue the returned hosted URL, e.g.:
      // this.tenantService.uploadLogo(file).subscribe(res => {
      //   this.logoPreviewUrl = res.url;
      //   this.tenantForm.patchValue({ logoUrl: res.url });
      // });
      this.tenantForm.patchValue({ logoUrl: dataUrl });
      this.tenantForm.get('logoUrl')?.markAsDirty();
    };
    reader.onerror = () => {
      this.logoFileError = 'Failed to read the selected file.';
      this.logoPreviewUrl = null;
      this.tenantForm.patchValue({ logoUrl: '' });
    };
    reader.readAsDataURL(file);
  }

  removeLogo(fileInput: HTMLInputElement) {
    this.logoPreviewUrl = null;
    this.logoFileError = null;
    this.tenantForm.patchValue({ logoUrl: '' });
    fileInput.value = '';
  }

  private loadTimeZones() {
    // Intl.supportedValuesOf is available in modern evergreen browsers
    // (Chrome 99+, Firefox 93+, Safari 15.4+). Fall back to a short
    // curated list if unsupported.
    let zones: string[];
    try {
      zones = (Intl as any).supportedValuesOf
        ? (Intl as any).supportedValuesOf('timeZone')
        : this.fallbackTimeZones();
    } catch {
      zones = this.fallbackTimeZones();
    }

    this.timeZones = zones
      .map((tz) => ({ value: tz, label: this.formatTimeZoneLabel(tz) }))
      .sort((a, b) => a.label.localeCompare(b.label));
  }

  private fallbackTimeZones(): string[] {
    return [
      'UTC', 'Europe/London', 'Europe/Berlin', 'Europe/Paris',
      'America/New_York', 'America/Chicago', 'America/Los_Angeles',
      'Asia/Kolkata', 'Asia/Dubai', 'Asia/Singapore', 'Asia/Tokyo',
      'Australia/Sydney'
    ];
  }

  private formatTimeZoneLabel(tz: string): string {
    try {
      const now = new Date();
      const offset = new Intl.DateTimeFormat('en-GB', {
        timeZone: tz,
        timeZoneName: 'shortOffset'
      })
        .formatToParts(now)
        .find((p) => p.type === 'timeZoneName')?.value ?? '';
      return `${tz.replace(/_/g, ' ')} (${offset})`;
    } catch {
      return tz.replace(/_/g, ' ');
    }
  }

  private loadCurrencies() {
    // A practical, commonly-used ISO 4217 currency set. Intl doesn't expose
    // an official "list all currencies" API, so this is curated rather than
    // derived — but labels are generated via Intl.DisplayNames so names stay
    // accurate/localized.
    const codes = [
      'GBP', 'USD', 'EUR', 'INR', 'JPY', 'AUD', 'CAD', 'CHF',
      'CNY', 'SGD', 'AED', 'ZAR', 'NZD', 'SEK', 'NOK'
    ];

    let displayNames: Intl.DisplayNames | null = null;
    try {
      displayNames = new Intl.DisplayNames(['en'], { type: 'currency' });
    } catch {
      displayNames = null;
    }

    this.currencies = codes
      .map((code) => ({
        value: code,
        label: displayNames ? `${code} — ${displayNames.of(code)}` : code
      }))
      .sort((a, b) => a.value.localeCompare(b.value));
  }

}